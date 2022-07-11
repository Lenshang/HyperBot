from fastapi import Body, FastAPI, Request, Response,requests, Request, status
from typing import Dict, Optional
from fastapi.responses import JSONResponse
from fastapi.exceptions import RequestValidationError
from pydantic import BaseModel
import uvicorn
import requests
app = FastAPI()
class MsgItem(BaseModel):
    #机器人QQ
    MQ_robot: Optional[str] = None
    MQ_type: Optional[int] = None
    MQ_type_sub: Optional[int] = None
    #群号
    MQ_fromID: Optional[str] = None
    MQ_fromQQ: Optional[str] = None
    MQ_passiveQQ: Optional[str] = None
    #正文
    MQ_msg: Optional[str] = None
    MQ_msgSeq: Optional[str] = None
    MQ_msgID: Optional[str] = None
    MQ_msgData: Optional[str] = None
    MQ_timestamp: Optional[str] = None

@app.get("/")
def root():
    return "HyperBot MYQQ Adapter"

@app.post("/get")
def get(body=Body(...)):
    msg_item=MsgItem.parse_raw(body)
    requests.post("")
    return "OK"


# @app.exception_handler(RequestValidationError)
# def validation_exception_handler(request: Request, exc: RequestValidationError):
#     #RequestValidationError输出的内容太累赘，很多重复的东西，我这里只取干货。
#     message = ""
#     for error in exc.errors():
#         message += ".".join(error.get("loc")) + ":" + error.get("msg") + ";"
    
#     return ""

if __name__ == '__main__':
    uvicorn.run(app, host="127.0.0.1", port=8765)