<?php
        /*
         *      Conner Knutson
         *      33295734
         *      Assignment 1
         *
         *      http://lyle.smu.edu/~cknutson/ass1/getMove.php 
         */

        $jsonString = file_get_contents("php://input");
        $myJson = json_decode($jsonString,true);

        $boardSize = $myJson['boardSize'];//save boardSize

        $pieces = $myJson['pieces'];//save pieces into array
        $enemy = $myJson['enemy'];//save enemy pieces into array

        $destinations = $myJson['destinations'];//save destinations into array

        //$frozen = $myJson['frozen'];//save frozen pieces into array

        $aimx = $destinations[0]["x"];
        $aimy = $destinations[0]["y"];
        for($a = 0; $a < count($destinations); $a++){
                for($b = 0; $b < count($pieces); $b++){
                        if($destinations[$a]["x"] == $pieces[$b]["x"]
                                && $destinations[$a]["y"] == $pieces[$b]["y"]){

                                unset($destinations[$a]);
                        }
                }
        }

        $xdir = array();//array for direction to move horizontally
        $ydir = array();//array for direction to move vertically
        //$distance = array();
        for($i = 0; $i < count($pieces); $i++){
                $j = $i;
                if($j >= count($destinations)){
                        $j = $j - count($destinations);//because destinations are not as many as pieces
                }
                $xdir[$i] = $destinations[0]["x"] - $pieces[$i]["x"];//if positive, piece should move right, increment x
                $ydir[$i] = $destinations[0]["y"] - $pieces[$i]["y"];//if negative, piece should move up, decrement y
                //$distance[$i] = abs($xdir[$i]) + abs($ydir[$i]);//distance approximation for each piece
        }

        $canMove = 0;

        //while($canMove == 0){//change condition on this loop, needs to execute for each piece and save piece's new location to array
        for($i = 0; $i < count($pieces); $i++){
                $canMove = 0;//0 if the current piece cannot move, 1 if it can
                $canMoveH = 0;//if it can move horizontally
                $canJumpH = 0;//if it can jump horizontally
                $canMoveV = 0;//if it can move vertically
                $canJumpV = 0;//if it can jump vertically
                $canMoveD = 0;//if it can move diagonally
                $canJumpD = 0;//if it can jump diagonally

                //$indexOfFurthest = array_keys($distance, max($distance));//array of indices of pieces that are the furthest away

                $currentPieceX = $pieces[$i]["x"];//x position of piece to move
                $currentPieceY = $pieces[$i]["y"];//y position of piece to move
		$currentPieceF = $pieces[$i]["frozen"];//whether the piece is frozen

                //do we want to move left, right, or neither
                $shiftH = 0;
                if($xdir[$i] > 0){
                        $shiftH = 1;//move right
                }
                elseif($xdir[$i] == 0){
                        $shiftH = 0;//don't move left or right
                }
                elseif($xdir[$i] < 0){
                        $shiftH = -1;//move left
                }

                //do we want to move up, down, or neither
                $shiftV = 0;
                if($ydir[$i] > 0){
                        $shiftV = 1;//move down
                }
                elseif($ydir[$i] == 0){
                        $shiftV = 0;//don't move up or down
                }
                elseif($ydir[$i] < 0){
                        $shiftV = -1;//move up
                }

                //check if the piece can move horizontally
                if(findPieceInBoard($pieces, $boardSize, $currentPieceX+$shiftH, $currentPieceY) == -1
                        && findPieceInBoard($enemy, $boardSize, $currentPieceX+$shiftH, $currentPieceY) == -1
                        && $currentPieceX+$shiftH >= 0
                        && $currentPieceX+$shiftH <= $boardSize-1){

                        $canMoveH = 1;
                        $canMove = 1;
                }

                //check if the piece can jump horizontally
                elseif(findPieceInBoard($pieces, $boardSize, $currentPieceX+(2*$shiftH), $currentPieceY) == -1
                        && findPieceInBoard($enemy, $boardSize, $currentPieceX+(2*$shiftH), $currentPieceY) == -1
                        && $currentPieceX+(2*$shiftH) >= 0
                        && $currentPieceX+(2*$shiftH) <= $boardSize-1){

                        $canJumpH = 1;
                        $canMove = 1;
                }

                //check if the piece can move vertically
                if(findPieceInBoard($pieces, $boardSize, $currentPieceX, $currentPieceY+$shiftV) == -1
                        && findPieceInBoard($enemy, $boardSize, $currentPieceX, $currentPieceY+$shiftV) == -1
                        && $currentPieceY+$shiftV >= 0
                        && $currentPieceY+$shiftV <= $boardSize-1){

                        $canMoveV = 1;
                        $canMove = 1;
                }

                //check if the piece can jump vertically
                elseif(findPieceInBoard($pieces, $boardSize, $currentPieceX, $currentPieceY+(2*$shiftV)) == -1
                        && findPieceInBoard($enemy, $boardSize, $currentPieceX, $currentPieceY+(2*$shiftV)) == -1
                        && $currentPieceY+(2*$shiftV) >= 0
                        && $currentPieceY+(2*$shiftV) <= $boardSize-1){

                        $canJumpV = 1;
                        $canMove = 1;
                }

                //check if the piece can move diagonally
                if(findPieceInBoard($pieces, $boardSize, $currentPieceX+$shiftH, $currentPieceY+$shiftV) == -1
                        && findPieceInBoard($enemy, $boardSize, $currentPieceX+$shiftH, $currentPieceY+$shiftV) == -1
                        && $currentPieceX+$shiftH >= 0
                        && $currentPieceX+$shiftH <= $boardSize-1
                        && $currentPieceY+$shiftV >= 0
                        && $currentPieceY+$shiftV <= $boardSize-1){

                        $canMoveD = 1;
                        $canMove = 1;
                }

                //check if the piece can jump diagonally
                elseif(findPieceInBoard($pieces, $boardSize, $currentPieceX+(2*$shiftH), $currentPieceY+(2*$shiftV)) == -1
                        && findPieceInBoard($enemy, $boardSize, $currentPieceX+(2*$shiftH), $currentPieceY+(2*$shiftV)) == -1
                        && $currentPieceX+(2*$shiftH) >= 0
                        && $currentPieceX+(2*$shiftH) <= $boardSize-1
                        && $currentPieceY+(2*$shiftV) >= 0
                        && $currentPieceY+(2*$shiftV) <= $boardSize-1){

                        $canJumpD = 1;
                        $canMove = 1;
                }

                if($canMove != 0 && $currentPieceF == 0){
                        //call method to calculate where piece should go
                        //change location of piece in array
                        $newPieceLocation = calcMove($i, $canMoveH, $canJumpH, $canMoveV, $canJumpV, $canMoveD, $canJumpD, $shiftH, $shiftV, $currentPieceX, $currentPieceY);
			$pieces[$i]["x"] = $newPieceLocation["x"];
			$pieces[$i]["y"] = $newPieceLocation["y"];
                }
        }

        //send pieces array back to UI
        echo json_encode($pieces);

function calcMove($i, $canMoveH, $canJumpH, $canMoveV, $canJumpV, $canMoveD, $canJumpD, $shiftH, $shiftV, $currentPieceX, $currentPieceY){
        $newX = 0;
        $newY = 0;

        //decide which direction (horizontal, vertical, diagonal) to move the piece in
        //jump if possible
        if($canJumpD == 1){
                $newX = $currentPieceX + (2*$shiftH);//jump x
                $newY = $currentPieceY + (2*$shiftV);//jump y
        }
        elseif($canJumpH == 1){
                $newX = $currentPieceX + (2*$shiftH);//jump x
                $newY = $currentPieceY;//don't move y
        }
        elseif($canJumpV == 1){
                $newX = $currentPieceX;//don't move x
                $newY = $currentPieceY + (2*$shiftV);//jump y
        }
        elseif($canMoveD == 1){
                $newX = $currentPieceX + $shiftH;//move x
                $newY = $currentPieceY + $shiftV;//move y
        }
        elseif($canMoveH == 1){
                $newX = $currentPieceX + $shiftH;//move x
                $newY = $currentPieceY;//don't move y
        }
        elseif($canMoveV == 1){
                $newX = $currentPieceX;//don't move x
                $newY = $currentPieceY + $shiftV;//move y
        }

	$toReturn = array();
	$toReturn["x"] = $newX;
	$toReturn["y"] = $newY;
	return $toReturn;

        //change piece's location in pieces array
        /*$pieces[$i]["x"] = $newX;
        $pieces[$i]["y"] = $newY;*/
}

        /*//create json
        $from = array();
        $from["x"] = $currentPieceX;
        $from["y"] = $currentPieceY;
        $to = array();
        $to["x"] = $newX;
        $to["y"] = $newY;
        $returnArray = array();
        $returnArray["from"] = $from;
        $returnArray["to"] = array($to);
        echo json_encode($returnArray);
        */
        /*
         *      Determines if a piece is located at a given set of coordinates
         *      
         *      @param  array   $board  contains the locations of the pieces on the board
         *      @param  integer $size   size of the board
         *      @param  integer $x              x coordinate to check
         *      @param  integer $y              y coordinate to check
         *      @return integer
         */
        function findPieceInBoard($board, $size, $x, $y){
        for($i = 0; $i < $size; $i++){
                if($board[$i]["x"] == $x && $board[$i]["y"] == $y){
                        return $i;
                }
        }
        return -1;
        }

        /*
        isFrozen
        Determines if a piece is frozen
        @param  integer $x the x coordinate to check
        @param  integer $y the y coordinate to check
        @param  array   $fBoard the array of all frozen pieces
        @return bool    whether the given coordinates match a frozen piece
        */
        function isFrozen($x, $y) {
                foreach($fBoard as $pc) {
                if($pc["x"] == $x && $pc["y"] == $y)
                        return true;
                }
                return false;
        }


?>
