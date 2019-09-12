function AutoPrint(){
    
    this.doPrint = function(json){
        if( typeof json == 'object' ){
            json = JSON.stringify(json);
        }
        var xhr = new XMLHttpRequest();
        xhr.upload.addEventListener("progress", this.ajaxRequestProgress, false); //设置上传进度监控
        xhr.onreadystatechange = function() {
            if (xhr.readyState == 4 ) {
                if( xhr.status==200 ){
                    
                }else{
                    console.error('Network Error!');
                }
            }
        };
        xhr.open("POST", "http://192.168.10.115:65432/", true);        // method,url,async
        xhr.setRequestHeader("Content-Type", 'application/json;charset=utf-8');
        //xhr.setRequestHeader("Origin", location.origin);
        // default content-type : multipart/form-data
        //                        application/x-www-form-urlencoded
        //xhr.withCredentials = true;
        xhr.send(json);
    }

    this.ajaxRequestProgress = function(evt) {  
        if (evt.lengthComputable) {  
            var percentComplete = Math.round(evt.loaded * 100 / evt.total);
        }else {  
            console.log("unable to compute");
        }
    }

    this.error = function(){

    }

    this.formatTime = function(timestamp) {
        var date = new Date(timestamp);
        var year = date.getFullYear()
        var month = date.getMonth() + 1
        var day = date.getDate()
        var hour = date.getHours()
        var minute = date.getMinutes()
        var second = date.getSeconds()
        return [year, month, day].map(this.formatNumber).join('-') + ' ' + [hour, minute].map(this.formatNumber).join(':');
    }

    this.formatNumber = function(n) {
        n = n.toString()
        return n[1] ? n : '0' + n
    }
}

var a = {
	set:{       /// 单位mm
		print:"58MBIII",
        file:'',
		width: 60,
        height:0,   // 0不限高
		left:5,
		top:5,
		right:5,
		bottom:5,
        fontSize:10,
        fontName:'',
        color:''
	},
	body:[
		{       // 单位px
            type:0, // 0文本，1横行
			text:"第一行第一行第一行第一行第一行第一行第一行第一行第一行第一行第一行第一行第一行第一行第一行第一行第一行第一行第一行",
			left:0,
			top:5,
			width:200,
			height:20,
            fontSize:10,
            fontName:'',
            color:''
		}
	]
}



var _print = new AutoPrint();
function doPrintOrder(order)
{
    let store = localStorage.getItem('store.print.orders');
    store == null ? store = [] : store = store.split(',');
    if (store.indexOf(order.id + '') >= 0) {
        return;
    }
    store.push(order.id);
    if (store.length > 1000) {
        store = store.splice(0, store.length - 1000);
    }
    localStorage.setItem('store.print.orders', store);

    if( !order.orderGoodsDetailList ){
        return;
    }
    
    console.log(order);
    console.log(order.orderGoodsDetailList);

    let waimai = 0;
    order.orderGoodsDetailList.forEach((obj) => {
        if (obj.categoryId == 3 || obj.categoryId == 4) {
            waimai += 1;
        }
    });

    var set = {
        set:{print:"58MBIII", file:'#'+order.id+'订单打印'+order.orderNum, width: 60, height:0, left:5, top:50, right:5, bottom:50, fontSize:9 },
        body:[]
    };

    if (order.orderType === '2' || order.orderType == 2 || waimai == 0) {
        var marginTop = 180;	// 上边距，px

        set.body.push({left:0 , top:50 , width:200, height:30, fontSize:18, text:'# '+order.id});
        set.body.push({left:0 , top:80, width:200, height:0, type:1});
        set.body.push({left:0 , top:85, width:200, height:30, text:'钱江高尔夫俱乐部'});

        set.body.push({left:0 , top:100, width:200, height:15, text:'订单号  ' + order.orderNum});
        set.body.push({left:0 , top:115, width:200, height:15, text:'下单时间' + order.createDateString});
        set.body.push({left:0 , top:130, width:200, height:15, text:'姓名：' + (order.linkName ? order.linkName : (order.realName ? order.realName : '-/-')) + "  " + (order.linkMobile ? order.linkMobile : (order.mobile ? order.mobile : ''))});
        set.body.push({left:0 , top:145, width:200, height:15, text:'台号：' + (order.linkAddress ? order.linkAddress : '-/-')});

        set.body.push({left:0 , top:160, width:100, height:15, text:'商品名称'});
        set.body.push({left:110, top:160, width:100, height:15, text:'数量'});
        set.body.push({left:150, top:160, width:100, height:15, text:'单价'});
        set.body.push({left:0 , top:175, width:200, height:0, type:1});

        var height = 0;
        // 订单基本信息

        // 订单商品列表   //ADD_PRINT_TEXT(intTop,intLeft,intWidth,intHeight,strContent);
        order.orderGoodsDetailList.forEach((obj, i) => {
            let top = (marginTop + i * 30);
            if (obj.categoryId != 3 && obj.categoryId != 4) {
                set.body.push({left:0 , top:top, width:110, height:30, text:obj.goodsTitle + '(非实体)'});
            } else {
                set.body.push({left:0 , top:top, width:110, height:30, text:obj.goodsTitle});
            }
            set.body.push({left:110 , top:top, width:50, height:30, text:obj.realGoodsNum});
            set.body.push({left:135 , top:top, width:60, height:30, text:(obj.price).toFixed(2)});

            height = top + 30;
        });
        set.body.push({left:0 , top:height, width:200, height:0, type:1});

        height += 5;
        set.body.push({left:0 , top:height, width:200, height:15, text:'消费合计：' + (order.totalMoney).toFixed(2)});
        if (order.totalMoney - order.afterMoney > 0) {
            height += 15;
            set.body.push({left:0 , top:height, width:200, height:15, text:'折扣优惠：' + (order.totalMoney - order.afterMoney).toFixed(2)});
        }
        if (order.cardMoney > 0) {
            height += 15;
            set.body.push({left:0 , top:height, width:200, height:15, text:'会员卡：' + (order.cardMoney).toFixed(2)});
        }
        height += 15;
        set.body.push({left:0 , top:height, width:200, height:15, text:'应付金额：' + (order.afterMoney).toFixed(2)});
        height += 15;
        set.body.push({left:0 , top:height, width:200, height:0, type:1});
        height += 5;
        set.body.push({left:0 , top:height, width:200, height:15, text:'打印时间：' + _print.formatTime(new Date().getTime())});
        //height += 100;
        //set.body.push({left:0 , top:height, width:200, height:0, text:'   '});
    }
    else {
        var marginTop = 145;	// 上边距，mm
        set.body.push({left:0 , top:50 , width:200, height:30, fontSize:18, text:'# '+order.id + ' 外卖'});
        set.body.push({left:0 , top:80, width:200, height:0, type:1});
        set.body.push({left:0 , top:85, width:200, height:30, text:'钱江高尔夫俱乐部'});


        set.body.push({left:0 , top:100, width:200, height:15, text:'订单号  ' + order.orderNum});
        set.body.push({left:0 , top:115, width:200, height:15, text:'下单时间' + order.createDateString});
        set.body.push({left:0 , top:130, width:100, height:15, text:'商品名称'});
        set.body.push({left:110, top:130, width:100, height:15, text:'数量'});
        set.body.push({left:150, top:130, width:100, height:15, text:'单价'});
        set.body.push({left:0 , top:145, width:200, height:0, type:1});

        var height = 0;
        // 订单商品列表   //ADD_PRINT_TEXT(intTop,intLeft,intWidth,intHeight,strContent);
        order.orderGoodsDetailList.forEach((obj, i) => {
            let top = (marginTop + i * 30);
            if (obj.categoryId != 3 && obj.categoryId != 4) {
                set.body.push({left:0 , top:top, width:110, height:30, text:obj.goodsTitle + '(非实体)'});
            } else {
                set.body.push({left:0 , top:top, width:110, height:30, text:obj.goodsTitle});
            }
            set.body.push({left:110 , top:top, width:50, height:30, text:obj.realGoodsNum});
            set.body.push({left:135 , top:top, width:60, height:30, text:(obj.price).toFixed(2)});

            height = top + 30;
        });
        set.body.push({left:0 , top:height, width:200, height:0, type:1});

        height += 5;
        set.body.push({left:0 , top:height, width:200, height:15, text:'消费合计：' + (order.totalMoney).toFixed(2)});
        if (order.totalMoney - order.afterMoney > 0) {
            height += 15;
            set.body.push({left:0 , top:height, width:200, height:15, text:'折扣优惠：' + (order.totalMoney - order.afterMoney).toFixed(2)});
        }
        if (order.cardMoney > 0) {
            height += 15;
            set.body.push({left:0 , top:height, width:200, height:15, text:'会员卡：' + (order.cardMoney).toFixed(2)});
        }
        height += 15;
        set.body.push({left:0 , top:height, width:200, height:15, text:'应付金额：' + (order.afterMoney).toFixed(2)});
        height += 15;
        set.body.push({left:0 , top:height, width:200, height:0, type:1});
        height += 5;
        set.body.push({left:0 , top:height, width:200, height:15, text:'打印时间：' + _print.formatTime(new Date().getTime())});
        height += 15;
        set.body.push({left:0 , top:height, width:200, height:20, fontSize:14, text:(order.linkName ? order.linkName : (order.realName ? order.realName : '-/-')) + "" + (order.linkMobile ? order.linkMobile : (order.mobile ? order.mobile : ''))});
        height += 25;
        set.body.push({left:0 , top:height, width:200, height:75, fontSize:14, text:(order.linkAddress ? order.linkAddress : '-/-')});
        //height += 100;
        //set.body.push({left:0 , top:height, width:200, height:0, text:'   '});
    }

    _print.doPrint(set);

    doPrintLabel(order);
}

function doPrintLabel(order){
    var set = {
        set:{print:"HPRT HY58", file:'#'+order.id+'订单商品标签打印'+order.orderNum, width: 30, height:20, left:5, top:5, right:0, bottom:0, fontSize:9 },
        body:[]
    };

    order.orderGoodsDetailList.forEach((obj, i) => {
        if (obj.categoryId == 3 || obj.categoryId == 4) {
            !obj.realGoodsNum ? obj.realGoodsNum=1:true;
            for(let i=0;i<obj.realGoodsNum;i++){
                set.body = [];
                set.body.push({left:0 , top:5, width:50, height:12, text:'# '+order.id});
                set.body.push({left:0 , top:20, width:120, height:40, text:obj.goodsTitle});
                _print.doPrint(set);
            }
        }
    });
}


function doPrintTest(){
    var _58MBIIII = {"set":{"print":"58MBIII","file":"#183订单打印20190909101635620218","width":60,"height":0,"left":5,"top":50,"right":5,"bottom":50,"fontSize":9},"body":[{"left":0,"top":50,"width":200,"height":30,"fontSize":18,"text":"# 183 外卖"},{"left":0,"top":80,"width":200,"height":0,"type":1},{"left":0,"top":85,"width":200,"height":30,"text":"钱江高尔夫俱乐部"},{"left":0,"top":100,"width":200,"height":15,"text":"订单号  20190909101635620218"},{"left":0,"top":115,"width":200,"height":15,"text":"下单时间2019-09-09 10:16:30"},{"left":0,"top":130,"width":100,"height":15,"text":"商品名称"},{"left":110,"top":130,"width":100,"height":15,"text":"数量"},{"left":150,"top":130,"width":100,"height":15,"text":"单价"},{"left":0,"top":145,"width":200,"height":0,"type":1},{"left":0,"top":145,"width":110,"height":30,"text":"奶茶(常温)"},{"left":110,"top":145,"width":50,"height":30,"text":1},{"left":135,"top":145,"width":60,"height":30,"text":"22.00"},{"left":0,"top":175,"width":110,"height":30,"text":"哈尔滨啤酒"},{"left":110,"top":175,"width":50,"height":30,"text":2},{"left":135,"top":175,"width":60,"height":30,"text":"10.00"},{"left":0,"top":205,"width":200,"height":0,"type":1},{"left":0,"top":210,"width":200,"height":15,"text":"消费合计：32.00"},{"left":0,"top":225,"width":200,"height":15,"text":"应付金额：32.00"},{"left":0,"top":240,"width":200,"height":0,"type":1},{"left":0,"top":245,"width":200,"height":15,"text":"打印时间：2019-09-12 12:00"},{"left":0,"top":260,"width":200,"height":20,"fontSize":14,"text":"柯先生18268821000"},{"left":0,"top":285,"width":200,"height":75,"fontSize":14,"text":"浙江省,杭州市,江干区 密度桥路1号20楼"},{"left":0,"top":295,"width":200,"height":0,"type":1}]};
    var HY58 = {"set":{"print":"HPRT HY58","file":"#183订单商品标签打印20190909101635620218","width":30,"height":20,"left":5,"top":5,"right":0,"bottom":0,"fontSize":9},"body":[{"left":0,"top":5,"width":50,"height":12,"text":"# 183"},{"left":0,"top":20,"width":120,"height":40,"text":"奶茶(常温)"}]};

    _print.doPrint(_58MBIIII);
    _print.doPrint(HY58);
}
