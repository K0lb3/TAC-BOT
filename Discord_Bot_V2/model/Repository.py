#This is the model for the repositories which will store all the resources we need to retrieve
import redis


class Repository:
    repo = None
    def __init__(self):
        self.repo = redis.Redis(
            host='localhost',
            port=6379)

    def loadJson(self): #we load the json file
        return self
    def converToUnits(self): #we batch convert them into units object
        return self 
    def storeInRedis(self): #we store them in a redis server hahAA
        return self
