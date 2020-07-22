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

# a simple page that says hello
@bp.route('/play')
def game():
    #return 'Game!'
    return render_template("game/play.html")