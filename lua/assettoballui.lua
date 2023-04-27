local ballPos = vec3(0,0,0)

local function signedAngleBetweenVectors(vec1, vec2)
    local dotProduct = vec1.x * vec2.x + vec1.y * vec2.y + vec1.z * vec2.z
    local magnitude1 = math.sqrt(vec1.x * vec1.x + vec1.y * vec1.y + vec1.z * vec1.z)
    local magnitude2 = math.sqrt(vec2.x * vec2.x + vec2.y * vec2.y + vec2.z * vec2.z)

    local angle = math.acos(dotProduct / (magnitude1 * magnitude2)) * 180 / math.pi

    -- Cross product
    local crossProduct = {
        x = vec1.y * vec2.z - vec1.z * vec2.y,
        y = vec1.z * vec2.x - vec1.x * vec2.z,
        z = vec1.x * vec2.y - vec1.y * vec2.x
    }

    -- Check if the angle is positive or negative
    if crossProduct.z < 0 then
        angle = -angle
    end

    return angle
end


local function angleToBall(cameraPos, cameraForward, ballPos)
    local vecToBall = {
        x = ballPos.x - cameraPos.x,
        y = ballPos.y - cameraPos.y,
        z = ballPos.z - cameraPos.z
    }

    return signedAngleBetweenVectors(cameraForward, vecToBall)
end

local function ballIsVisible()
    local cameraPos = ac.getCameraPosition()
    local cameraForward = ac.getCameraForward()

    local angle = angleToBall(cameraPos, cameraForward, ballPos)
    local fov = ac.getCameraFOV()

    return angle < fov / 2
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
        local cameraPos = ac.getCameraPosition()
        local cameraForward = ac.getCameraForward()

        local angle = angleToBall(cameraPos, cameraForward, ballPos)

        -- Calculate the center point of the screen
        local centerX = ui.windowWidth() / 2
        local centerY = ui.windowHeight() / 2

        -- Calculate the endpoint of the arrow
        local arrowLength = 100
        local endpointX = centerX + math.cos(math.rad(angle)) * arrowLength
        local endpointY = centerY - math.sin(math.rad(angle)) * arrowLength

        ui.drawLine(vec2(centerX, centerY), vec2(endpointX, endpointY), rgbm(1, 1, 1, 1), 2)
        ui.drawTriangleFilled(vec2(endpointX - 10, endpointY - 10), vec2(endpointX - 10, endpointY + 10), vec2(endpointX + 10, endpointY), rgbm(1, 1, 1, 1))

        ui.sameLine()
        ui.pushFont(ui.Font.Huge)
        ui.text("Angle to ball: " .. angle)
        ui.popFont()
    end
end
