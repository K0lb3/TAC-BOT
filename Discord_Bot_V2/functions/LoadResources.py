import os
import json

def LoadResources():
    mypath = os.path.dirname(os.path.realpath(__file__)).replace('functions','resources')
    files = os.listdir(mypath)

    ret={}

    for f in files:
        print(f)
        with open(os.path.join(mypath,f), "rt", encoding='utf8') as file:
            ret[f[:-5]]=json.loads(file.read())
            
    return ret



def LoadUnits():
    path = '/home/slashershot/Desktop/TAC-BOT/Discord_Bot_V2/resources/Unit.json'
    ret = None
    with open(path,"rt",encoding='utf8') as file:
        ret = json.loads(file.read())
    
    return ret
    
