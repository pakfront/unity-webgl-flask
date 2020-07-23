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
        "SELECT id, gamename, created, summary"
        " FROM game"
        " ORDER BY created DESC"
    ).fetchall()



    players = {}
    for game in games:
        players[game['id']] = db.execute(
        "SELECT username"
        " FROM player p"
        " JOIN user u" 
        " ON p.user_id = u.id"
        " WHERE game_id = (?)",(game['id'],),
        ).fetchall()

    openseats = {}
    for game in games:
        openseats[game['id']] = db.execute(
        "SELECT id, slot"
        " FROM player p"
        " WHERE p.user_id IS null AND game_id = (?)",(game['id'],),
        ).fetchall()

    if request.args.get('type') == 'json':
        # return jsonify(games)
        return jsonify(openseats)

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
        "SELECT p.id, gamename, p.user_id, slot"
        " FROM player p"
        " JOIN game g" 
        " ON p.game_id = g.id"
    ).fetchall()

    if request.args.get('type') == 'json':
        return jsonify(players)
    return render_template("intro/index.html",players=players)
