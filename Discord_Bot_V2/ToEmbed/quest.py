# from ToEmbed._main import DIRS,LinkDB,ConvertFields,StrBuff,StrCondition,Embed
# from PIL import Image, ImageFont, ImageDraw
# import io
#
# ##### SETTINGS ################################################################################
# DEBUG=False  #show printed map on command
# TILE_RESOLUTION=48
# BORDER_WIDTH=1
# HEIGHTSTEPS = 13
# FONT={
#     'TEXT':     ImageFont.truetype(font='ebrima.ttf', size=int(TILE_RESOLUTION/4), index=0, encoding=''),
#     'HEADER':   ImageFont.truetype(font='ebrima.ttf', size=int(TILE_RESOLUTION/3), index=0, encoding='')
# }
# COLOR={
#     'PARTY':        (65,105,225),
#     'ALLY':         (58,190,98),
#     'ENEMY':        (166,16,30),
#     'TREASURE':     (249,224,0),
#     'JEWEL':        (255,153,255),
#     'BLOCKED':      (139,137,137),
#     'HEIGHT_MIN':   (245,245,220),
#     'HEIGHT_MAX':   (139,69,19),
#     'HEADER':       (0,0,0),
#     'HEADER_BORDER':(255,255,255),
#     'BORDER':       (0,0,0),
#     'TEXT':         (0,0,0),
#     'TEXT_HEADER':  (255,255,255),
# }
#
# ##### CONSTANTS #######################################################
# UNIT=DIRS['Enemy']
# UNIT.update(DIRS['Unit'])
#
# ##### QUEST PAGE DECISION #############################################
# def Quest(iname,page):
#     quest=DIRS['Quests'][iname]
#     SET=quest['map'][0]['Set']
#
#     #create basic embed
#     embed= Embed(
#         title=quest['name'], #page name
#         #url=LinkDB('quest',iname)  #page link
#         )
#
#     if page=='drop':
#         fields=[
#             {'name':    'AP',       'value':    quest['ap'],  'inline':True},
#             {'name':    'Enemies',  'value':    str(len(SET['enemy'])),  'inline':True},
#             {'name':    'Chests',   'value':    str(len(SET['treasure'])),  'inline':True},
#             {'name':    'Drops',       'value': '\n'.join([
#                 DIRS[item]['name']
#                 for item in quest['dropList']
#                 ]), 'inline':True}
#         ]
#         embed.ConvertFields(fields)
#         return(embed,False)
#
#     if page=='main':
#         sides=['ally','enemy','jewel']
#         fields=[
#             {'name': side.title(), 'value': '\n'.join([
#                 '{num} ~ {name}'.format(num=i+1, name=UNIT[unit['iname']]['name'])
#                 for i,unit in enumerate(SET[side])
#             ]), 'inline':True}
#             for side in sides
#             if SET[side]
#         ]
#         embed.ConvertFields(fields)
#         MAP=MapImage(quest['map'][0])
#         return(embed,MAP)
#
#
# ##### CREATE MAP IMAGE '''''''''''''''''''''''''''''''''''''''''''''''''''''''''
# def MapImage(MAP):
#     #settings
#     tile_res=TILE_RESOLUTION #resolution of a tile
#     header = COLOR['HEADER'] #color for headers
#     header_b=COLOR['HEADER_BORDER'] #color for header border
#     border={
#         'use':      True,
#         'color':    COLOR['BORDER'],
#         'width':    BORDER_WIDTH
#         }
#
#     #DATA
#     Scene=MAP['Scene']
#     Set=MAP['Set']
#     width=Scene['w']
#     height=Scene['h']
#
#     #COLOR GRADIENT FOR HEIGHT
#     global grad
#     grad = ColorGradient(COLOR['HEIGHT_MIN'],COLOR['HEIGHT_MAX'],HEIGHTSTEPS)
#
#     #CONVERT ARRAY TO MATRIX
#     AIO=[
#         [
#             {
#                 'text': 'H'+str(Scene['grid'][y*width+x]['h']),
#                 'height': Scene['grid'][y*width+x]['h'],
#                 'type': 'blocked' if 'tile' in Scene['grid'][y*width+x] else 'tile'
#             }
#             for x in range(width)
#         ]
#         for y in range(height)
#     ]
#
#     #ADD SPAWNS TO MAP
#     def placeSpawn(unit,number,typ):
#         tile=AIO[unit['pos']['y']][unit['pos']['x']]
#         tile.update({
#             'text': '{pre}{unit}~H{height}'.format(unit=number,pre=typ[0].upper(),height= tile['height']),
#             'type': typ
#             })
#
#     sides=['party','ally','enemy','treasure','jewel']
#     for side in sides:
#         for i,unit in enumerate(Set[side]):
#             placeSpawn(unit,i+1,side)
#
# ##### DRAW IMAGE ##################################################################################################
#     #upsize and add borders
#     data=[]
#     #top header
#     border['length']=border['width']*width + (width+1)*tile_res
#     for x in range(width+1):
#         data+=[header]*tile_res + ([header_b]*border['width'] if x!=width else [])
#     data+= data*(tile_res-1) + [header_b]*border['length']
#
#     #define horizontal border
#     border_hori=[header_b]*(tile_res+border['width'])+[border['color']]*(border['length']-border['width']-tile_res)
#     border_hori+=border_hori*(border['width']-1)
#     #
#     for y in range(height):
#         #add row header with header border
#         line=[header]*tile_res + [header_b]*border['width']
#         for x in range(width):
#             #add normal tiles with border
#             line+=[Coloring(AIO[y][x])]*tile_res + ([border['color']]*border['width'] if x+1!=width else [])
#         #add line multiple times for the right height
#         data+=line*tile_res + ( border_hori if y+1!=height else [])
#     #define image
#     img = Image.new('RGB', ((width+1)*tile_res+(width)*border['width'], (height+1)*tile_res+(height)*border['width']), "white")
#     #add data
#     img.putdata(data)
#
# ##### HEADER   #####################################################
#     AIO.insert(0,[
#         {
#             'text': chr(65+i),
#             'type': 'Header'
#         }
#         for i in range(width)
#     ])
#     AIO[0].insert(0,{'text':'\\','type':'Header'})
#     for y in range(1,height+1):
#         AIO[y].insert(0,{
#             'text': str(y),
#             'type': 'Header'
#         })
#
# ##### WRITE TEXT ########################################################################
#     scale=tile_res
#
#     draw=ImageDraw.Draw(img)
#     for y in range(height+1):
#         for x in range(width+1):
#             if AIO[y][x]['type'] == 'Header':
#                 tcolor=COLOR['TEXT_HEADER']
#                 tfont=FONT['HEADER']
#             else:
#                 tcolor=COLOR['TEXT']
#                 tfont=FONT['TEXT']
#             w, h = draw.textsize(AIO[y][x]['text'],tfont)
#             px=x*(scale+border['width']) + int(scale/2 - w/2)
#             py=y*(scale+border['width']) + int(scale/2 - h/2)
#             draw.text(
#                 (px,py),  # Coordinates
#                 AIO[y][x]['text'],  # Text
#                 tcolor,
#                 tfont
#                 )
#
#     #img.save('my.png')
#     if DEBUG:
#         img.show()
#
#     #convert to binary
#     imgByteArr = io.BytesIO()
#     img.save(imgByteArr, format='PNG')
#     imgByteArr = imgByteArr.getvalue()
#     return imgByteArr
#
# def ColorGradient(c0,cE,n):
#     grow=[
#         int((c0[i]-cE[i])/(n-1))
#         for i in range(len(c0))
#     ]
#
#     return [
#         (
#             (c0[0]-i*grow[0]),     #Red
#             (c0[1]-i*grow[1]),     #Green
#             (c0[2]-i*grow[2])      #Blue
#         )
#         for i in range(0,n)
#     ]
#
# def Coloring(tile):
#     if tile['type']=='tile':
#         return(grad[tile['height']])
#     return(COLOR[tile['type'].upper()])
