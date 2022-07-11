import json
import base64
from urllib import parse
class HyBot:
    def __init__(self,name,version,description="") -> None:
        self.cmd_list={}
        self.host=""
        self.name=name
        self.version=str(version)
        self.description=description
        self.task_cache=[]
    def reg(self,cmd,func):
        self.cmd_list[cmd]=func

    def get_all_cmd(self):
        return list(self.cmd_list.keys())

    def write(self,item):
        send_str=parse.quote(json.dumps(item))
        print(send_str)

    def send_msg(self,msg,group_id=None):
        self.task_cache.append({
            "group_id":group_id,
            "msg":msg
        })
    def run(self):
        while True:
            try:
                raw_str=input()
                if raw_str[:raw_str.find(":")]=="$init":
                    self.host=raw_str[raw_str.find(":")+1:]
                    item={
                        "cmd_list":self.get_all_cmd(),
                        "name":self.name,
                        "version":self.version,
                        "description":self.description
                    }
                    self.write(item)
                if raw_str[:raw_str.find(":")]=="$cmd":
                    raw_item=raw_str[raw_str.find(":")+1:]
                    item=json.loads(raw_item)
                    cmd=item["cmd"]
                    msg=item["msg"]
                    return_msg=self.cmd_list[cmd](msg)
                    resp_item={
                        "msg":return_msg,
                        "tasks":self.task_cache
                    }
                    self.write(resp_item)

                    self.task_cache.clear()
            except:
                pass