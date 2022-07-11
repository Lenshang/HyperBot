from PyHyBot.HyBot import HyBot

class test(HyBot):
    def __init__(self) -> None:
        super().__init__("test_py", 0.1)
    def test(self,item):
        self.send_msg("test_send")
        return "abc"
    def start(self):
        self.reg("testpy",self.test)
        self.run()

if __name__ == '__main__':
    test().start()