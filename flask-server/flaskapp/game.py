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

bp = Blueprint("game", __name__, url_prefix="/game")

@bp.route('/play')
@login_required
def game():
    return render_template("game/play.html")

@bp.route("/<int:id>/join") #, methods=("GET", "POST"))
@login_required
def join(id):
    db = get_db()
    # game = db.execute(
    #     "SELECT username"
    #     " FROM player p"
    #     " JOIN user u" 
    #     " ON p.user_id = u.id"
    #     " WHERE game_id = (?)",(id,),
    #     ).fetchall()

    player = db.execute(
        "SELECT p.id, slot, gamename, p.user_id"
        " FROM player p"
        " JOIN game g"
        " ON p.game_id = g.id"
        " WHERE p.id = (?)",(id,),
        ).fetchone()

    if player['user_id'] is not None:
        return 'Error: Slot already filled'

    user_id = g.user['id']    
    db.execute(
       "UPDATE player"
       " SET user_id = ?"
       " WHERE id = ?",(user_id, id,),
    )

    db.commit()

    player = db.execute(
        "SELECT p.id, slot, gamename, p.user_id"
        " FROM player p"
        " JOIN game g"
        " ON p.game_id = g.id"
        " WHERE p.id = (?)",(id,),
        ).fetchone()
    return jsonify(player)

    