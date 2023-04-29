local ballPos = vec3(0,0,0)

local GameState = {
    WAITING = 0,
    PLAYING = 1,
    END = 2
}

local function vec3_cross(a, b)
    return vec3(
        a.y * b.z - a.z * b.y,
        a.z * b.x - a.x * b.z,
        a.x * b.y - a.y * b.x
    )
end


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
    local vecToBallXZ = {
        x = ballPos.x - cameraPos.x,
        z = ballPos.z - cameraPos.z
    }

    local cameraForwardXZ = {
        x = cameraForward.x,
        z = cameraForward.z
    }

    local angle = signedAngleBetweenVectorsXZ(cameraForwardXZ, vecToBallXZ)
    angle = angle + 90 -- Add an offset of 90 degrees

    return angle
end


local function ballIsVisible()
    local cameraPos = ac.getCameraPosition()
    local cameraForward = ac.getCameraForward()

    local angle = angleToBallXZ(cameraPos, cameraForward, ballPos)
    local fov = ac.getCameraFOV()

    -- Calculate the angle threshold based on the camera FOV
    local angleThreshold = fov * 0.5  -- 90% of half of the FOV (experiment with the 0.9 factor)

    return math.abs(angle) < angleThreshold
end


local function drawArrowToBall()
    local cameraPos = ac.getCameraPosition()
    local cameraForward = ac.getCameraForward()

    local angle = angleToBallXZ(cameraPos, cameraForward, ballPos)

    -- Calculate the center point of the screen
    local centerX = ui.windowWidth() / 2
    local centerY = ui.windowHeight() / 2

    -- Calculate the endpoint of the arrow
    local arrowLength = 100
    local endpointX = centerX + math.cos(math.rad(angle)) * arrowLength
    local endpointY = centerY - math.sin(math.rad(angle)) * arrowLength

    ui.drawLine(vec2(centerX, centerY), vec2(endpointX, endpointY), rgbm(1, 1, 1, 1), 2)
    ui.drawTriangleFilled(vec2(endpointX - 10, endpointY - 10), vec2(endpointX - 10, endpointY + 10), vec2(endpointX + 10, endpointY), rgbm(1, 1, 1, 1))
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
