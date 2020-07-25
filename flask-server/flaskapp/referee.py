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

@bp.route("/<int:id>/turndata", methods=("GET", "POST"))
@login_required
def turndata(id):
    #TODO make sure g.user is a referee
    db = get_db()
    game = db.execute(
        "SELECT g.id, gamename, turnnum, turndata, referee_id"
        " FROM game g"
        " WHERE g.id = (?)",(id,),
        ).fetchone()

    if game["referee_id"] != g.user["id"]:
        abort(403)

    if request.method == "POST":
        turndata = request.form["turndata"]
        error = None

        if not turndata:
            error = "turndata is required."

        if error is not None:
            flash(error)
        else:
            db = get_db()
            db.execute(
                "UPDATE game SET turndata = ? WHERE id = ?", (commanddata, id)
            )
            db.commit()
            return "Submitted turndata for "+game['gamename']
            # return redirect(url_for("blog.index"))

    return jsonify(game)

@bp.route("/<int:id>/commanddata", methods=("GET", "POST"))
@login_required
def commanddata(id):
    #TODO make sure g.user is a referee
    db = get_db()

    game = db.execute(
        "SELECT g.id, gamename, referee_id"
        " FROM game g"
        " WHERE g.id = (?)",(id,),
        ).fetchone()

    if game["referee_id"] != g.user["id"]:
        abort(403)
        
    players = db.execute(
        "SELECT p.id, playernumber, commanddata, p.user_id, p.game_id"
        " FROM player p"
        " WHERE p.game_id = (?)",(id,),
        ).fetchall()

    return jsonify(players)
    # return jsonify([tuple(row) for row in player[0]])

