from flask import Blueprint
from flask import flash
from flask import g
from flask import redirect
from flask import render_template
from flask import request
from flask import url_for
from werkzeug.exceptions import abort

from flaskapp.auth import login_required
from flaskapp.db import get_db

from flask import jsonify


bp = Blueprint("intro", __name__)


@bp.route("/")
def index():
    db = get_db()
    games = db.execute(
        "SELECT g.id, gamename, g.created, summary, username, referee_id"
        " FROM game g"
        " JOIN user u" 
        " ON g.referee_id = u.id"
        " ORDER BY g.created DESC"
    ).fetchall()



    players = {}
    for game in games:
        players[game['id']] = db.execute(
        "SELECT p.id, playernumber, username, user_id, turnnum"
        " FROM player p"
        " JOIN user u" 
        " JOIN game g" 
        " ON p.user_id = u.id AND p.game_id = g.id"
        " WHERE game_id = (?)",(game['id'],),
        ).fetchall()

    openseats = {}
    for game in games:
        openseats[game['id']] = db.execute(
        "SELECT id, playernumber"
        " FROM player p"
        " WHERE p.user_id IS null AND game_id = (?)",(game['id'],),
        ).fetchall()

    if request.args.get('type') == 'json':
        # return jsonify(games)
        return jsonify(openseats)
    print(players)
    print(openseats)

    return render_template("intro/index.html",games=games, players=players, openseats=openseats)
    
@bp.route("/players")
def players():
    db = get_db()
    # players = db.execute(
    #     "SELECT gamename, username"
    #     " FROM player p"
    #     " JOIN game g" 
    #     " JOIN user u" 
    #     " ON p.game_id = g.id AND p.user_id = u.id"
    # ).fetchall()

    players = db.execute(
        "SELECT p.id, gamename, p.user_id, playernumber"
        " FROM player p"
        " JOIN game g" 
        " ON p.game_id = g.id"
    ).fetchall()

    if request.args.get('type') == 'json':
        return jsonify(players)
    return render_template("intro/index.html",players=players)

@bp.route("/myplayers")
def myplayers():
    db = get_db()
    players = db.execute(
        "SELECT p.id, gamename, p.user_id, playernumber"
        " FROM player p"
        " JOIN game g" 
        " ON p.user_id = ? AND p.game_id = g.id",(g.user['id'],),
    ).fetchall()

    return jsonify( {"Items":players} )

@bp.route("/mygames")
def mygames():
    db = get_db()
    games = db.execute(
        "SELECT g.id, gamename, turnnum"
        " FROM game g"
        " WHERE g.referee_id = ?",(g.user['id'],),
    ).fetchall()

    return jsonify( {"Items":games} )

    # if request.args.get('type') == 'json':
        # return jsonify(players)
    # return render_template("intro/index.html",players=players)