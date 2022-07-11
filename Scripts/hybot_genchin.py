from PyHyBot.HyBot import HyBot

class Genshin(HyBot):
    def __init__(self) -> None:
        super().__init__("genshin", 0.1)
        self.once_cost=160
    def help(self,item):
        return ("原神抽卡系统\n" +
        f"单次抽卡只需{self.once_cost}金币! 10连仅需{self.once_cost*10}金币\n" +
        "[可选指令]\n" +
        "#原神单抽 卡池名称\n" +
        "#原神十连 卡池名称\n" +
        "#原神50连 卡池名称\n" +
        "#原神90连 卡池名称\n" +
        "[可选卡池]\n" +
        "常驻,角色,武器\n" +
        "[最新资讯]\n" +
        "金币不够的可以先输入 #打很长时间的工 获得金币哟!")

    def start(self):
        self.reg("原神抽卡",self.help)
        self.run()

if __name__ == '__main__':
    Genshin().start()