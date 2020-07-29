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

bp = Blueprint("referee", __name__, url_prefix="/referee")


@bp.route("/game/<int:id>/commanddata")
@login_required
def commanddata(id):
    db = get_db()

    game = db.execute(
        "SELECT g.id, gamename, referee_id"
        " FROM game g"
        " WHERE g.id = (?)",(id,),
        ).fetchone()

    if game["referee_id"] != g.user["id"]:
        abort(403)
        
    players = db.execute(
        "SELECT p.id, commanddata"
        " FROM player p"
        " WHERE p.game_id = (?)",(id,),
        ).fetchall()

    return jsonify({"Items":players})
    # return jsonify([tuple(row) for row in player[0]])

@bp.route("/game/<int:id>/turndata", methods=("GET", "POST"))
@login_required
def turndata(id):
    db = get_db()

    game = db.execute(
        "SELECT id, referee_id, turndata, turnnum, gamename"
        " FROM game g"
        " WHERE g.id = (?)",(id,),
        ).fetchone()

    if game["referee_id"] != g.user["id"]:
        abort(403)

    if request.method == "POST":
        turndata = request.form["turndata"]
        turnnum = int(request.form["turnnum"])
        error = None

        if not turndata:
            error = "turndata is required."
        if not turnnum:
            error = "turnnum is required."
        currentturnnum = int(game["turnnum"])
        if turnnum <= currentturnnum:
            error = "turnnum "+turnnum+" is <= game turnnum "+currentturnnum

        if error is not None:
            flash(error)
        else:
            db = get_db()
            db.execute(
                "UPDATE game SET turndata = ?, turnnum = ? WHERE id = ?", (turndata, turnnum, id)
            )
            db.commit()
            return "Submitted turndata for "+game['gamename']
            # return redirect(url_for("blog.index"))

    return jsonify(game)

@bp.route("/player/<int:id>/viewdata", methods=("GET", "POST"))
@login_required
def viewdata(id):
    db = get_db()

    db = get_db()
    player = db.execute(
        "SELECT p.id, playernumber, username, referee_id"
        " FROM player p"
        " JOIN game g" 
        " JOIN user u" 
        " ON p.game_id = g.id AND p.user_id = u.id"
        " WHERE p.id = (?)",(id,),
        ).fetchone() 

    if player["referee_id"] != g.user["id"]:
        abort(403)
        
    if request.method == "POST":
        viewdata = request.form["viewdata"]
        error = None

        if not viewdata:
            error = "viewdata is required."

        if error is not None:
            flash(error)
        else:
            db = get_db()
            db.execute(
                "UPDATE player SET viewdata = ? WHERE id = ?", (viewdata, id)
            )
            db.commit()
            return "Submitted viewdata for "+player['username']

    return jsonify(player)



