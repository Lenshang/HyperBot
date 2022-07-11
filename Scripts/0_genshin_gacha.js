import GenshinGacha from "./utils/genshin_gacha.js";

/**
 * 原神抽卡模块
 */
(function(){
    var core=Hyper.getCoreModule();
    var once_cost=160;
    var genshin_gacha=new GenshinGacha();
    Hyper.reg("原神抽卡", new CommandAction((v, group, user) => {
        return "原神抽卡系统\n" +
        `单次抽卡只需${once_cost}金币! 10连仅需${once_cost*10}金币\n` +
        "[可选指令]\n" +
        "#原神单抽 卡池名称\n" +
        "#原神十连 卡池名称\n" +
        "#原神50连 卡池名称\n" +
        "#原神90连 卡池名称\n" +
        "[可选卡池]\n" +
        "常驻,角色,武器\n" +
        "[最新资讯]\n" +
        "金币不够的可以先输入 #打很长时间的工 获得金币哟!";
    }));

    Hyper.reg("原神单抽",new CommandAction((v, group, user)=>{
        var coreUser=core.GetUser(group,user);
        if (coreUser.Gold < 160){
            return `${coreUser.QQName}呀,你的钱,还不够呀!`;
        }

    }));

    Hyper.reg("原神测试", new CommandAction((v, group, user) => {
        let coreUser=core.GetUser(group,user);
        if(!coreUser.JSStorage.ContainsKey("genshin")){
            coreUser.JSStorage.Item.set("genshin",{
                items:[],
                last_gacha:DateTime.Now
            });
        }
        let store = coreUser.JSStorage.Item.get("genshin");
        store.last_gacha=DateTime.Now;
        return store.last_gacha.ToString();
    }));
})()