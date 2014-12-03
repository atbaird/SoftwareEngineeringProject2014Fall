/* V 2.2
 *  adds team property to Cell. default is -1 for just a Cell
 *  team will be 0,1,.. the index into gTeamList
 *  todo: create internal player
 * @type Number
 */

var kBoardWidth = 18; //change to 0 to use with Initialization form
var kBoardHeight = 18; //change to 0 to use with Initialization form
var kPieceWidth = 20;
var kPieceHeight = 20;
var kPixelWidth = 1 + (kBoardWidth * kPieceWidth); // change to 0 to use with initialization form
var kPixelHeight = 1 + (kBoardHeight * kPieceHeight); // change to 0 to use with initialization form
var destinationCorner = "upperRight"; //change to "" to use with initialization form
var piecesCorner = "lowerLeft"; //change to "" to user with initialization form

var gCanvasElement;
var gDrawingContext;
var gPattern;

var gPieces = [];  // so we can push and append
var gDestinations;
var gNumDests;
var gSelectedPieceIndex;
var gSelectedPieceHasMoved;
var gMoveCount;
var gMoveCountElem;
var gGameInProgress;


// v 2.0
var gTeamList = []; // filled in newGame - array Of Teams
var gTurnCount = 0;
var gNumTeams  = 2; 

var url = "";  // ??

function Team(teamIdx, startArea, destArea, color) {
    this.teamIdx = teamIdx;
    this.startArea = startArea;
    this.destArea  = destArea;
    this.color     = color;
    this.teamPieces = [];
    this.teamDestinations = [];
    this.teamUrl    = '';
    this.sendPostNoParm = true;  // changed if python AI ends with py
    this.name = "nonames";       // comes in from HTML 
}

function Cell(y, x, f) {
    this.y = y;
    this.x = x;
    this.team = -1;  // none
    this.frozen = f;
}
/*function CellJSON(y, x, t) {
    this.y = y;
    this.x = x;
    this.team = t;
}*/

function isThereAPieceBetween(cell1, cell2) {
    /* note: assumes cell1 and cell2 are 2 squares away
     either vertically, horizontally, or diagonally */
    var rowBetween = (cell1.y + cell2.y) / 2;
    var columnBetween = (cell1.x + cell2.x) / 2;
    for (var i = 0; i < gPieces.length; i++) {
        if ((gPieces[i].y === rowBetween) &&
                (gPieces[i].x === columnBetween)) {
            return true;
        }
    }
    return false;
}

function isTheGameOver() {
    for (var i = 0; i < gPieces.length; i++) {
        if (gPieces[i].y > 2) {
            return false;
        }
        if (gPieces[i].x < (kBoardWidth - 3)) {
            return false;
        }
    }
    return true;
}

function drawBoard() {
    if (gGameInProgress && isTheGameOver()) {
        endGame();
    }

    gDrawingContext.clearRect(0, 0, kPixelWidth, kPixelHeight);

    gDrawingContext.beginPath();

    /* vertical lines */
    for (var x = 0; x <= kPixelWidth; x += kPieceWidth) {
        gDrawingContext.moveTo(0.5 + x, 0);
        gDrawingContext.lineTo(0.5 + x, kPixelHeight);
    }

    /* horizontal lines */
    for (var y = 0; y <= kPixelHeight; y += kPieceHeight) {
        gDrawingContext.moveTo(0, 0.5 + y);
        gDrawingContext.lineTo(kPixelWidth, 0.5 + y);
    }

    /* draw it! */
    gDrawingContext.strokeStyle = "#ccc";
    gDrawingContext.stroke();

    /* todo: add teams  */
    for (var i = 0; i < gPieces.length; i++) {
        drawPiece(gPieces[i], (i === gSelectedPieceIndex));
    }

    gMoveCountElem.innerHTML = gMoveCount;

    saveGameState();
}

function drawPiece(p, selected) {
    var column = p.x;
    var row    = p.y;
    //console.log("draw Piece at Row: " + row + ", Col: " + column);
    var x = (column * kPieceWidth) + (kPieceWidth / 2);
    var y = (row * kPieceHeight) + (kPieceHeight / 2);
    var radius = (kPieceWidth / 2) - (kPieceWidth / 10);
    gDrawingContext.beginPath();
    gDrawingContext.arc(x, y, radius, 0, Math.PI * 2, false);
    gDrawingContext.closePath();
    gDrawingContext.strokeStyle = "#000";
    gDrawingContext.stroke();
    
    // find the team piece p is part of and use team color
    // 
    // version 1.2
    fillColor = gTeamList[p.team].color;
    
    /* ver 2.0   removed when we added team property to Cell
    var found = false;
    for(var i = 0; i < gTeamList[0].teamPieces.length; i++) {
      if (teamPieces[i].x === p.x && teamPieces[i].y === p.y ) {
        found = true;
        fillColor = gTeamList[0].color;
        break;
      }
    }  // end loop over team0 pieces
    
    // piece must be on other team if not found
    if (!found) fillColor = gTeamList[1].color;
    
    */
    
    // fill in team color
    gDrawingContext.fillStyle = fillColor;
    gDrawingContext.fill();
        
    // draw current piece black
    if(selected) {
        gDrawingContext.fillStyle = "#000";
        gDrawingContext.fill();
        
        // try interior colored circle
        gDrawingContext.beginPath();
        gDrawingContext.arc(x, y, radius/2, 0, Math.PI * 2, false);
        gDrawingContext.closePath();
        gDrawingContext.strokeStyle = "#000";
        gDrawingContext.stroke();
        // fill in team color
        gDrawingContext.fillStyle = fillColor;
        gDrawingContext.fill();
    }

    
    
    // todo: draw cells in destination area differently
    // assume destination areas are upperRight and upperLeft

    if ((column < 3 && row < 3) || 
        (column >= kBoardWidth - 3 && row < 3)  )   {
        gDrawingContext.fillStyle = "#00ff00";
        gDrawingContext.fill();
        
        // draw inner circle?
    }

}

if (typeof resumeGame !== "function") {
    saveGameState = function() {
        return false;
    };
    resumeGame = function() {
        return false;
    };
}

function newGame() {
    // set up team that knows origin("lowerLeft or lowerRight,
    //                             destintion, color, url or localJSopponent
    // based on that each team gets its pieces
    // which get appended via push to gPieces
    // global gTeamList holds Team instances
    
    // set up teams
    var team0 = new Team(0, "lowerLeft", "upperRight", "#CC0099");
    var team1 = new Team(1, "lowerRight", "upperLeft", "#006699");
  
    gTeamList[0] = team0;
    gTeamList[1] = team1;
    
    var url1 = document.getElementById("team1Url").value;
    var url2 = document.getElementById("team2Url").value;
    
    // TEAMS : URLs SET - if AI is python; send json with board=  as parm
    gTeamList[0].url = url1;
    if (endsWith(url1, "py")) gTeamList[0].sendPostNoParm = false;
    console.log("team send parm with POST = " + gTeamList[1].sendPostNoParm);
    gTeamList[0].name = document.getElementById("team1Name").value;
    
    gTeamList[1].url = url2;  //could be localjs
    if (endsWith(url1, "py")) gTeamList[1].sendPostNoParm = false;
    gTeamList[1].name = document.getElementById("team2Name").value;
    
    for ( i=0; i<gTeamList.length; i++) {
        
        // setUpTeamPieces
        // setUpTeamDestinations
        // return array filled with cells from a corner
        teamPieceArr = setUpTeamPieces(gTeamList[i].startArea);
        teamDestArr  = setUpTeamDestinations(gTeamList[i].destArea);
        gTeamList[i].teamPieces      = teamPieceArr;
        gTeamList[i].teamDestinations = teamDestArr;
        
        // add team pieces to gPieces -- game engine sees just pieces
        for (k=0; k<teamPieceArr.length; k++) {
            // team pieces now know their team (0 or 1)
            teamPieceArr[k].team = i; // i is teamIdx
            gPieces.push(teamPieceArr[k]);
        }

    }
    
    //gNumDests = gDestinations.length;  //??
    gSelectedPieceIndex = -1;
    gSelectedPieceHasMoved = false;
    gMoveCount = 0;
    gGameInProgress = true;
    //alert("ready to draw gPieces length = " + gPieces.length);
    drawBoard();
}

// returns array of pieces (Cells) for a team
// the Cells should be added to gPieces

function setUpTeamPieces(piecesCorner) {
    
    if (piecesCorner === "lowerLeft") {
        teamPieces = [
            new Cell(kBoardHeight - 4, 0,0),
            new Cell(kBoardHeight - 3, 0,0),
            new Cell(kBoardHeight - 2, 0,0),
            new Cell(kBoardHeight - 1, 0,0),
            new Cell(kBoardHeight - 4, 1,0),
            new Cell(kBoardHeight - 3, 1,0),
            new Cell(kBoardHeight - 2, 1,0),
            new Cell(kBoardHeight - 1, 1,0),
            new Cell(kBoardHeight - 4, 2,0),
            new Cell(kBoardHeight - 3, 2,0),
            new Cell(kBoardHeight - 2, 2,0),
            new Cell(kBoardHeight - 1, 2,0)];
    }

    else if (piecesCorner === "lowerRight") {
        teamPieces = [
            new Cell(kBoardHeight - 4, kBoardWidth - 3,0),
            new Cell(kBoardHeight - 3, kBoardWidth - 3,0),
            new Cell(kBoardHeight - 2, kBoardWidth - 3,0),
            new Cell(kBoardHeight - 1, kBoardWidth - 3,0),
            new Cell(kBoardHeight - 4, kBoardWidth - 2,0),
            new Cell(kBoardHeight - 3, kBoardWidth - 2,0),
            new Cell(kBoardHeight - 2, kBoardWidth - 2,0),
            new Cell(kBoardHeight - 1, kBoardWidth - 2,0),
            new Cell(kBoardHeight - 4, kBoardWidth - 1,0),
            new Cell(kBoardHeight - 3, kBoardWidth - 1,0),
            new Cell(kBoardHeight - 2, kBoardWidth - 1,0),
            new Cell(kBoardHeight - 1, kBoardWidth - 1,0)];
    }
    else alert("setUpTeamPieces does not understand: " + piecesCorner);
    
    return teamPieces;
}
  function setUpTeamDestinations(destinationCorner) {  
    if (destinationCorner === "lowerLeft") {
        teamDestinations = [           
            new Cell(kBoardHeight - 1, 0,0),
            new Cell(kBoardHeight - 2, 0,0),
            new Cell(kBoardHeight - 3, 0,0),

            new Cell(kBoardHeight - 1, 1,0),
            new Cell(kBoardHeight - 2, 1,0),
            new Cell(kBoardHeight - 3, 1,0),

            new Cell(kBoardHeight - 1, 2,0),
            new Cell(kBoardHeight - 2, 2,0),
            new Cell(kBoardHeight - 3, 2,0)

        ];
    }
    else if (destinationCorner === "upperLeft") {
        teamDestinations = [new Cell(0, 0),
            new Cell(1, 0,0),
            new Cell(2, 0,0),
            new Cell(0, 1,0),
            new Cell(1, 1,0),
            new Cell(2, 1,0),
            new Cell(0, 2,0),
            new Cell(1, 2,0),
            new Cell(2, 2,0)];
    }
    else if (destinationCorner === "upperRight") {
        teamDestinations = [new Cell(0, kBoardWidth - 1),
            new Cell(1, kBoardWidth - 1,0),
            new Cell(2, kBoardWidth - 1,0),
            new Cell(0, kBoardWidth - 2,0),
            new Cell(1, kBoardWidth - 2,0),
            new Cell(2, kBoardWidth - 2,0),
            new Cell(0, kBoardWidth - 3,0),
            new Cell(1, kBoardWidth - 3,0),
            new Cell(2, kBoardWidth - 3,0)];
    }
    else if (destinationCorner === "lowerRight") {
        teamDestinations = [new Cell(kBoardHeight - 1, kBoardWidth - 1),
            new Cell(kBoardHeight - 2, kBoardWidth - 1,0),
            new Cell(kBoardHeight - 3, kBoardWidth - 1,0),
            new Cell(kBoardHeight - 1, kBoardWidth - 2,0),
            new Cell(kBoardHeight - 2, kBoardWidth - 2,0),
            new Cell(kBoardHeight - 3, kBoardWidth - 2,0),
            new Cell(kBoardHeight - 1, kBoardWidth - 3,0),
            new Cell(kBoardHeight - 2, kBoardWidth - 3,0),
            new Cell(kBoardHeight - 3, kBoardWidth - 3,0)];
    }
    else alert("setUpDestinations does not understand: " + destinationCorner);
    return teamDestinations;
}

function endGame() {
    gSelectedPieceIndex = -1;
    gGameInProgress = false;
}

function initGame(canvasElement, moveCountElement) {
    /***************************************************
     * Uncomment following code to use initialization form
     *****************************************************/
//    var boardSize = document.getElementById("boardSize").value;
//    kBoardHeight = boardSize;
//    kBoardWidth = boardSize;
//    kPixelWidth = 1 + (kBoardWidth * kPieceWidth);
//    kPixelHeight = 1 + (kBoardHeight * kPieceHeight);
//    
//    destinationCorner = $('input:radio[name=destCorner]:checked').val();
//    piecesCorner = $('input:radio[name=pieceCorner]:checked').val();

    
   
    if (!canvasElement) {
        canvasElement = document.createElement("canvas");
        canvasElement.id = "halma_canvas";
        document.body.appendChild(canvasElement);
    }
    if (!moveCountElement) {
        moveCountElement = document.createElement("p");
        document.body.appendChild(moveCountElement);
    }
    gCanvasElement = canvasElement;
    gCanvasElement.width = kPixelWidth;
    gCanvasElement.height = kPixelHeight;
    gMoveCountElem = moveCountElement;
    gDrawingContext = gCanvasElement.getContext("2d");
    if (!resumeGame()) {
        newGame();
    }
    $('#initialization').hide();
    $('#game').show();
}

function make20Moves() {
    for(var i=0; i<20; i++) {
       makeMove(); 
       if (isGameOver() )  break;
    }
} 
function siphonPostCollisionMoves(arr1, arr2) {
    //suppose to detect a collision, and if so, returns the location of the collision, while removing all other moves from the string.
    //pieces are frozen; value needs to be one more than actual number of turns they will be frozen for.
    var moves1 = arr1["to"];
    var moves2 = arr2["to"];
    var boolCatch = false;
    var collision = new Cell(-1,-1,0);
    for(var i = 0; i < moves1.length; i++) {
        for(var j = i; j < i+1 && j < moves2.length; j++) {
            if(gPieces[i].x == gPieces[j].x && gPieces[i].y == gPieces[j].y) {
                boolCatch = true;
                collision = moves1[i];
                moves1[i].frozen = 2;
                moves2[j].frozen = 2;
                var ran = Math.random() % 2;
                if(ran == 1) {
                    gPieces[i].x -= 1;
                } else if(ran == 0) {
                    gPieces[j].x +=1;
                }
            }
        }
    }
    if(moves1[moves1.length - 1].x == moves2[moves2.length-1].x && moves1[moves1.length-1].y == moves2[moves2.length-1].y) {
        boolCatch = true;
        collision = moves1[i];
        moves1[i].frozen = 2;
        moves2[j].frozen = 2;
        var ran = Math.random() % 2;
        if(ran == 1) {
            gPieces[i].x -= 1;
        } else if(ran == 0) {
            gPieces[j].x +=1;
        }
    }
    return collision;
}
function unFreeze(moves) {
    //suppose to decrement any frozen pieces' frozen value.
    for(var i = 0; i < moves.length; i++) {
        if(moves[i].frozen > 0) {
            moves[i].frozen--;
        }
    }
}
// 
// Called when MOVE button pressed
// todo: add turns
function makeMove() {
    
    if (isGameOver() ) return;
    
    gTurnCount++;
    var currentTeam = 0; 
    var currentTeam1 = 1;
    // get move for current team

    unFreeze(gTeamList[currentTeam]);
    unFreeze(gTeamList[currentTeam1]);
    var move = makeAjaxPostMoveRequestNoParm(currentTeam);
    var move1 = makeAjaxPostMoveRequestNoParm(currentTeam1);

    siphonPostCollisionMoves(move, move1);

      
    //fc: display incoming json and Team Name
    var teamSpan         = document.getElementById("AITeamName");
    teamSpan.innerHTML   = gTeamList[currentTeam].name;
    teamSpan.style.color = gTeamList[currentTeam].color;

    document.getElementById("responseString").innerHTML = JSON.stringify(move);

    var teamSpan1 = document.getElementById("AITeamName1");
    teamSpan1.innerHTML = gTeamList[currentTeam1].name;
    teamSpan.style.color = gTeamList[currentTeam1].color;

    document.getElementById("responseString1").innerHTML = JSON.stringify(move1);

        checkIfFoundPiece(move);
        checkIfFoundPiece(move1);
    }

        

    function checkIfFoundPiece(move) {
        // try and move 
        // is this a piece on current team? if not exit
        
        var locPiece = move.from;
        var currPieceLoc = new Cell(locPiece.y, locPiece.x, 0);
        
	console.log("currPieceLoc");
	console.log(currPieceLoc);
        var movePieceLocs = move.to; 

        // create moves - array of Cells where AI wants to move
        // todo: check if these are legal moves here
        // -- need isFromPieceOnTeam(currentTeamIdx)
        //         if length of toMoves >1 we must be jumping
        //    need isValidSingleMove(fromP, toP)
        //    need isValidJump(fromP, toP) with some piece between
        var moves = [];
        for(var i = 0; i < movePieceLocs.length; i++) {
            moves.push(new Cell(movePieceLocs[i].y, movePieceLocs[i].x, movePieceLocs[i].frozen));
        }

        var foundPiece = false;
        // search if desired move is for an actual piece
        // todo: change to on the team moving
        for (var i = 0; i < gPieces.length; i++) {
            if ((gPieces[i].y === currPieceLoc.y)
                    && (gPieces[i].x === currPieceLoc.x)) {
                gSelectedPieceIndex = i;
                // set piece location to last in chain
                // todo: hold off on this until we know the last move is valid
                gPieces[i].y = moves[moves.length - 1].y;
                gPieces[i].x = moves[moves.length - 1].x;
                gMoveCount += 1;
                foundPiece = true;
                break;
            }
        }
        
        // draw small dot where a jump sequence has happened
        // todo: check if the desired squares are really jumps
        if (foundPiece) {  // we are trying to move a valid piece
            drawBoard();
            // set up for drawing dot
            var x = (currPieceLoc.x * kPieceWidth) + (kPieceWidth / 2);
            var y = (currPieceLoc.y * kPieceHeight) + (kPieceHeight / 2);
            var radius = (kPieceWidth / 2) - (kPieceWidth / 10);
            gDrawingContext.beginPath();
            gDrawingContext.arc(x, y, radius / 3, 0, Math.PI * 2, false);
            gDrawingContext.closePath();
            gDrawingContext.strokeStyle = "#000";
            gDrawingContext.stroke();
            gDrawingContext.fillStyle = "#f00";
            gDrawingContext.fill();
            
            // draw all except last move as dot - draw piece in last loc
            for(var i = 0; i < moves.length - 1; i++) {
                var x = (moves[i].x * kPieceWidth) + (kPieceWidth / 2);
                var y = (moves[i].y * kPieceHeight) + (kPieceHeight / 2);
                var radius = (kPieceWidth / 2) - (kPieceWidth / 10);
                gDrawingContext.beginPath();
                gDrawingContext.arc(x, y, radius/3, 0, Math.PI * 2, false);
                gDrawingContext.closePath();
                gDrawingContext.strokeStyle = "#000";
                gDrawingContext.stroke();
                gDrawingContext.fillStyle = "#000";
                gDrawingContext.fill();
            }
        }
        else
            alert("No piece at requested move location: " + currPieceLoc.y
                    + ", " + currPieceLoc.x);
 
    }

function makeAjaxPostMoveRequestNoParm(teamIdx) {
    
    var move = "No move received. See Alerts.";  // overwrite w/ HTTP response
    
      $.ajax({
        type: 'POST',
        url: gTeamList[teamIdx].url,
        dataType: "json",
        async: false,
        data: boardToJSON(teamIdx),
        success: function(msg) {
            move = msg;     
        },
        error: function(jqXHR, exception) {
            if (jqXHR.status === 0) {
                alert('Unable to connect.\n Verify Network.');
            } else if (jqXHR.status === 404) {
                alert('Requested URL of HalmaAI not found. [404]');
            } else if (jqXHR.status === 500) {
                alert('Internal Server Error [500].');
            } else if (exception === 'parsererror') {
                alert('Data from HalmaAI was not JSON :( Parse failed.');
            } else if (exception === 'timeout') {
                alert('Time out error.');
            } else if (exception === 'abort') {
                alert('Ajax request aborted.');
            } else {
                alert('Uncaught Error.\n' + jqXHR.responseText);
            }
        }
        
      });
      
      return move;
}

function makeAjaxPostMoveRequestWithParm(teamIdx) {
    
var move; 

      $.ajax({
        type: 'POST',
        url: gTeamList[teamIdx].url,
        dataType: "json",
        async: false,
        data: {board: boardToJSON(teamIdx)},
        success: function(msg) {
            move = msg;     
        },
        error: function(jqXHR, exception) {
            if (jqXHR.status === 0) {
                alert('Unable to connect.\n Verify Network.');
            } else if (jqXHR.status == 404) {
                alert('Requested URL of HalmaAI not found. [404]');
            } else if (jqXHR.status == 500) {
                alert('Internal Server Error [500].');
            } else if (exception === 'parsererror') {
                alert('Data from HalmaAI was not JSON :( Parse failed.');
            } else if (exception === 'timeout') {
                alert('Time out error.');
            } else if (exception === 'abort') {
                alert('Ajax request aborted.');
            } else {
                alert('Uncaught Error.\n' + jqXHR.responseText);
            }
        }
        
      });
      
      return move;
}

function checkInputs() {
    
    var $myForm = $('#initForm');
    if (!$myForm[0].checkValidity()) {
        $myForm.find(':submit').click();
        return;
    }
    else {
        // calls new game and sets up teams
        initGame(null, document.getElementById('movecount'));
    }
}

$(document).ready(function() {
    $('#game').hide();
    $('#initialization').show();
    $('#NoParm').click();   // default for radio button POST option
    
    
});

// format  data based on team turn 
function boardToJSON(teamIdx) {
    var enemyIdx = 0;
    if(teamIdx===0) enemyIdx = 1;
    
    return JSON.stringify({
        "pieces" : gTeamList[teamIdx].teamPieces,
        "destinations" : gTeamList[teamIdx].teamDestinations,
        "boardSize" : kBoardHeight,
        "enemy" : gTeamList[enemyIdx].teamPieces,
        "enemydestinations" : gTeamList[enemyIdx].teamDestinations

    });
    
}

function endsWith(str, suffix) {
    return str.indexOf(suffix, str.length - suffix.length) !== -1;
}

function areAllDestinationsFilled(destArr, pieceArr) {
    
    for (var i=0; i< destArr.length; i++) {
        // is piece at this loc
       var destOccupied = false; // find only one
       for (var k=0; k<pieceArr.length; k++) {
          if (destArr[i].x === pieceArr[k].x &&
                destArr[i].y === pieceArr[k].y  )  {
            destOccupied = true;
            break;
          }
       }
       if(destOccupied === false) return false;
    }
    // we get this far it is true that all are occupied
    return true;
}

function isGameOver() {
    // for each team
    // check if all destination pieces are occupied by one of their pieces
    
    for (var i=0; i<gTeamList.length; i++) {
        if (areAllDestinationsFilled( gTeamList[i].teamDestinations,
                                      gTeamList[i].teamPieces)) {
            document.getElementById('winnerCircle').innerHTML =
                    "We have a WINNER: " + gTeamList[i].name;
            return true;
         }
        
    }
    // no winner if we get here
    return false;
    
}
