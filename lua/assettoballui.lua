local baseUrl = "http://" .. ac.getServerIP() .. ":" .. ac.getServerPortHTTP() .. "/AssettoBallPlugin/"

local ballPos = vec3(0, 0, 0)

local GameState = {
    WAITING = 0,
    PLAYING = 1,
    END = 2
}

--ballfunctoins

local function signedAngleBetweenVectorsXZ(vec1, vec2)
    local dotProduct = vec1.x * vec2.x + vec1.z * vec2.z
    local magnitude1 = math.sqrt(vec1.x * vec1.x + vec1.z * vec1.z)
    local magnitude2 = math.sqrt(vec2.x * vec2.x + vec2.z * vec2.z)

    local angle = math.acos(dotProduct / (magnitude1 * magnitude2)) * 180 / math.pi

    -- Cross product (Y component only)
    local crossProductY = vec1.x * vec2.z - vec1.z * vec2.x

    -- Check if the angle is positive or negative
    if crossProductY < 0 then
        angle = -angle
    end

    return angle
end


local function angleToBallXZ(cameraPos, cameraForward, ballPos)
    -- Calculate the vector from the camera to the ball in the XZ plane
    local vecToBallXZ = vec3(ballPos.x - cameraPos.x, 0, ballPos.z - cameraPos.z)

    -- Normalize the vectors for a pure direction comparison
    
    local normVecToBallXZ = vecToBallXZ:normalize()
    local normCameraForward = vec3(cameraForward.x, 0, cameraForward.z):normalize()

    -- Calculate the angle using atan2 for a full range of angles (-pi to pi)
    local angleRad = math.atan2(normVecToBallXZ.z, normVecToBallXZ.x) - math.atan2(normCameraForward.z, normCameraForward.x)

    -- Convert radians to degrees for UI calculations, adjusting 
    local angleDeg = angleRad * (180 / math.pi) - 90

    return angleDeg
end


local function ballIsVisible()
    local cameraPos = ac.getCameraPosition()
    local cameraForward = ac.getCameraForward()

    -- Use the original angle calculation without the 90-degree UI offset
    local angle = angleToBallXZ(cameraPos, cameraForward, ballPos)
    
    -- Correcting the visibility logic to account for the 90-degree offset indirectly
    local correctedAngle = angle + 90

    -- FOV check
    local fov = ac.getCameraFOV()
    -- Ensure the angle is within the FOV, taking into account the corrected orientation
    return math.abs(correctedAngle) <= fov / 2
end


local function drawArrowToBall()
    local cameraPos = ac.getCameraPosition()
    local cameraForward = ac.getCameraForward()

    -- Calculate the angle to the ball in the XZ plane
    local angle = angleToBallXZ(cameraPos, cameraForward, ballPos)
    
    -- UI properties
    local centerX = ui.windowWidth() / 2
    local centerY = ui.windowHeight() / 2
    local indicatorDistance = 50 -- Distance from screen center to draw the arrow
    local arrowSize = 20 -- Size of the arrow

    -- Calculate the base position of the arrow on the screen edge in the direction of the ball
    local baseX = centerX + indicatorDistance * math.cos(math.rad(angle))
    local baseY = centerY + indicatorDistance * math.sin(math.rad(angle))
    
    -- Calculate points for the arrow head
    local tipX = baseX + arrowSize * math.cos(math.rad(angle))
    local tipY = baseY + arrowSize * math.sin(math.rad(angle))
    local leftX = baseX + arrowSize * math.cos(math.rad(angle + 120))
    local leftY = baseY + arrowSize * math.sin(math.rad(angle + 120))
    local rightX = baseX + arrowSize * math.cos(math.rad(angle - 120))
    local rightY = baseY + arrowSize * math.sin(math.rad(angle - 120))

    -- Draw the arrow
    ui.drawLine(vec2(centerX, centerY), vec2(tipX, tipY), 2, rgb(255, 255, 255)) -- Line from base to tip
    ui.drawLine(vec2(leftX, leftY), vec2(tipX, tipY), 2, rgb(255, 255, 255)) -- Left side of arrow head
    ui.drawLine(vec2(rightX, rightY), vec2(tipX, tipY), 2, rgb(255, 255, 255)) -- Right side of arrow head
end


local function drawGameScore(score)
    -- function to draw the game score
end

local function drawTeams(teams)
    -- function to draw the teams
end

local function drawTimeLeft(timeLeft)
    -- function to draw the time left in the game
end



local assettoBallEvent = ac.OnlineEvent({
    ac.StructItem.key("assettoBallPacket"),
    PosX = ac.StructItem.float(),
    PosY = ac.StructItem.float(),
    PosZ = ac.StructItem.float(),
    VelX = ac.StructItem.float(),
    VelY = ac.StructItem.float(),
    VelZ = ac.StructItem.float()
}, function(sender, message)
    if sender ~= nil then return end
    ballPos = vec3(message.PosX, message.PosY, message.PosZ)
end)


function script.drawUI()
    local visible = ballIsVisible()

    if visible == false then
        drawArrowToBall()
    end

    drawGameScore()
    drawTeams()
    drawTimeLeft()
end
