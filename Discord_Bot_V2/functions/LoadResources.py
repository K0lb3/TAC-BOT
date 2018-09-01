import os
import json

def LoadResources():
    mypath = os.path.join(os.path.dirname(os.path.realpath(__file__)), '..', 'resources')
    with open (os.path.join(mypath, 'Database.json'), "rt", encoding='utf8') as file:
            ret=json.loads(file.read())
    return ret
