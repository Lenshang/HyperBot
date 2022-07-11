
export default class{
    constructor(){
        var data_str=Hyper.readFile("./data/genshin_data.json");
        var data=JSON.parse(data_str);
        this.data=data;
        this.cardDb=[];

        this.cardDb=this.cardDb.concat(this.data.常驻);
        this.cardDb=this.cardDb.concat(this.data.角色up);
        this.cardDb=this.cardDb.concat(this.data.武器up);
        this.cardDb=this.cardDb.concat(this.data.未开放);
        this.cardDb=this.cardDb.concat(this.data.未开放武器);
    }
    reload(){
        var data_str=Hyper.readFile("./data/genshin_data.json");
        var data=JSON.parse(data_str);
        this.data=data;
        this.cardDb=[];

        this.cardDb=this.cardDb.concat(this.data.常驻);
        this.cardDb=this.cardDb.concat(this.data.角色up);
        this.cardDb=this.cardDb.concat(this.data.武器up);
        this.cardDb=this.cardDb.concat(this.data.未开放);
        this.cardDb=this.cardDb.concat(this.data.未开放武器);
    }
    /**
     * 
     * @param {int} pool 1=常驻,2=角色UP,3=武器UP
     * @param {bool} baodi 是否只出4星或5星 一般用在10连没有4星或90连没有5星
     * @param {bool} isUp 当为UP池时如果出5星是否必定为UP角色
     */
    getOne(pool,baodi=0,isUp=false){
        var pr={}
        var cards=[];
        if(pool==1){
            pr=this.data.常驻概率;
            cards = this.data.常驻;
        }
        else if(pool==2){
            pr = this.data.角色up概率;
            cards=cards.concat(data.角色up);
            cards=cards.concat(data.角色up.filter(i=>i.Level==4));
            cards=cards.concat(data.角色up.filter(i=>i.Level==4));
            cards=cards.concat(data.角色up.filter(i=>i.Level==4));
            cards=cards.concat(data.常驻.filter(i=>i.Level==4));
        }
        else{
            pr = this.data.武器up概率;
            cards=cards.concat(data.武器up);
            cards=cards.concat(data.武器up.filter(i=>i.Level==4));
            cards=cards.concat(data.武器up.filter(i=>i.Level==4));
            cards=cards.concat(data.武器up.filter(i=>i.Level==4));
            cards=cards.concat(data.常驻.filter(i=>i.Level==4));
        }

        let rd=Hyper.getRandom(0, 1000);
        
        if (rd < pr["5"] * 1000||baodi==5){
            //如果抽中5星
            if (isUp|| Hyper.getRandom(0,2)==1){
                let _cards = cards.filter(i => i.Level == "5");
                return [true,_cards[Hyper.getRandom(0, _cards.length)]];
            }
            else if(pool == 2){
                let _cards = this.data.常驻.filter(i => i.Level == "5"&&i.Type== "character");
                return [false,_cards[Hyper.getRandom(0, _cards.length)]];
            }
            else if (pool == 3){
                let _cards = this.data.常驻.filter(i => i.Level == "5"&&i.Type== "weapon");
                return [false,_cards[Hyper.getRandom(0, _cards.length)]];
            }
            else{
                let _cards = this.data.常驻.filter(i => i.Level == "5");
                return [false,_cards[Hyper.getRandom(0, _cards.length)]];
            }
        }
        else if (rd < pr["4"] * 1000||baodi==4){
            //如果抽中4星
            let _cards = cards.filter(i => i.Level == "4");
            return [false,_cards[Hyper.getRandom(0, _cards.length)]];
        }
        else{
            //三星
            let _cards = cards.filter(i => i.Level == "3");
            return [false,_cards[Hyper.getRandom(0, _cards.length)]];
        }
    }

    get(pool,count,getcount){
        let result=[];
        has4 = false;
        for(let i = 0; i < getcount; i++){
            count+=1;
            let _hasup = false;
            if (count == 90){
                let r = this.getOne(pool, 5);
                _hasup=r[0];
                result.push(r[1]);
            }
            else if (count == 180){
                let r = this.getOne(pool, 5,true);
                _hasup=r[0];
                result.push(r[1]);
            }
            else if(i == 9&& !has4){
                let r = this.getOne(pool, 4);
                _hasup=r[0];
                result.push(r[1]);
            }
            else{
                let r = this.getOne(pool, 0);
                if(r.Level=="4"||r.Level=="5"){
                    has4=true
                }
                result.push(r)
            }

            if(result[i].Level == "5"&&!_hasup){
                count = 90;
            }
            if (_hasup)
            {
                count = 0;
            }
        }
        return [count,result];
    }
}