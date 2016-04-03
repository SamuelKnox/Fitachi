from flask import Flask
import time
import json
import random
app = Flask(__name__)

class Player:
    id = None
    hitPoints = 0
    attackPower = 0

class Match:
    id = 0
    player0 = None
    player1 = None
    turn = None

playersWaiting = []
matchesReady = {}

matchesWaiting = []
matches = []
matchCount = 0

#Default entry point for server
@app.route('/start')
def start():
    return "Launching server..."

#Finds the player a match, or enqueues them if none are available
@app.route('/find_match/<player>')
def findMatch(player):
    global playersWaiting
    searchingPlayer = createPlayerFromJson(player)
    if searchingPlayer not in playersWaiting:
        playersWaiting.append(searchingPlayer)
    return getMatchStatus(player)

#Checks if the match is ready to begin
@app.route('/get_match_status/<player>')
def getMatchStatus(player):
    global playersWaiting
    global matchCount
    searchingPlayer = createPlayerFromJson(player)
    if searchingPlayer.id in matchesReady:
        matchReady = matchesReady[searchingPlayer.id]
        for playerWaiting in playersWaiting:
            if playerWaiting.id == searchingPlayer.id:
                playersWaiting.remove(playerWaiting)
        del matchesReady[searchingPlayer.id]
        return getMatchJson(matchReady)
    for playerWaiting in playersWaiting:
        if playerWaiting.id == searchingPlayer.id or isMatched(playerWaiting, searchingPlayer):
            continue
        matchCount += 1
        match = Match()
        match.id = matchCount
        match.player0 = playerWaiting
        match.player1 = searchingPlayer
        match.turn = playerWaiting if random.choice([True, False]) else searchingPlayer
        matches.append(match)
        matchesReady[playerWaiting.id] = match
        for playerWaiting in playersWaiting:
            if playerWaiting.id == searchingPlayer.id:
                playersWaiting.remove(playerWaiting)
        return getMatchJson(match)
    invalidMatch = createInvalidMatch()
    print invalidMatch.id
    return getMatchJson(invalidMatch)

#gets the updated status for the match
@app.route('/update_match/<match>')
def updateMatch(match):
    global matches
    currentMatch = createMatchFromJson(match)
    for matchInstance in matches:
        if currentMatch.id == matchInstance.id:
            return getMatchJson(matchInstance)
    return getMatchJson(createInvalidMatch())

#Attacks for the player in the specified match
@app.route('/attack/<match>')
def attack(match):
    global matches
    matchInstance = createMatchFromJson(match)
    if matchInstance.player0.id == matchInstance.turn.id:
        matchInstance.turn = matchInstance.player1
        matchInstance.player1.hitPoints -= matchInstance.player0.attackPower
    else:
        matchInstance.turn = matchInstance.player0
        matchInstance.player0.hitPoints -= matchInstance.player1.attackPower
    for matchIteration in matches:
        if matchIteration.id == matchInstance.id:
            matches.remove(matchIteration)
            matches.append(matchInstance)
    return getMatchJson(matchInstance)

#checks if two players are currently in a match
def isMatched(player0, player1):
    global matches
    for match in matches:
        if match.player0.id == player0.id and match.player1.id == player1.id or match.player0.id == player1.id and match.player1.id == player0.id:
            return True
    return False

#creates an invalid match
def createInvalidMatch():
    match = Match()
    invalidPlayer = createInvalidPlayer()
    match.player0 = invalidPlayer
    match.player1 = invalidPlayer
    match.turn = invalidPlayer
    return match

#creates an invalid player
def createInvalidPlayer():
    player = Player()
    player.id = ""
    return player

#converts a match to json
def getMatchJson(match):
    matchData = {
        'id':match.id,
        'player0':
            {
            'id':match.player0.id,
            'hitPoints':match.player0.hitPoints,
            'attackPower':match.player0.attackPower,
            },
        'player1':
            {
            'id':match.player1.id,
            'hitPoints':match.player1.hitPoints,
            'attackPower':match.player1.attackPower,
            },
        'turn':
            {
            'id':match.turn.id,
            'hitPoints':match.turn.hitPoints,
            'attackPower':match.turn.attackPower,
            },
        }
    matchJson = json.dumps(matchData)
    return matchJson

#takes the json for a match and creates a match from it
def createMatchFromJson(match):
    matchJson = json.loads(match)
    newMatch = Match()
    newMatch.id = matchJson['id']
    player0Json = json.dumps(matchJson['player0'])
    player1Json = json.dumps(matchJson['player1'])
    turnJson =  json.dumps(matchJson['turn'])
    newMatch.player0 = createPlayerFromJson(player0Json)
    newMatch.player1 = createPlayerFromJson(player1Json)
    newMatch.turn = createPlayerFromJson(turnJson)
    return newMatch

#takes the json for a player and creates a player from it
def createPlayerFromJson(player):
    playerJson = json.loads(player)
    newPlayer = Player()
    newPlayer.id = playerJson['id']
    newPlayer.hitPoints = playerJson['hitPoints']
    newPlayer.attackPower = playerJson['attackPower']
    return newPlayer

if __name__ == "__main__":
    app.debug = True
    app.run(host='0.0.0.0')
