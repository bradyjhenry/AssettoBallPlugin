local ball = {} -- Assuming you have an object that you want to move smoothly
ball.position = {x = 0, y = 0, z = 0}
ball.targetPosition = {x = 0, y = 0, z = 0}

local got_message = false;

local message_in = nil;

local assettoBallEvent = ac.OnlineEvent({
    ac.StructItem.key("assettoBallPosition"),
    X = ac.StructItem.float(),
    Y = ac.StructItem.float(),
    Z = ac.StructItem.float()
}, function(sender, message)
    if sender ~= nil then return end
    got_message = true;
    message_in = message;
    ball.targetPosition = vec3(message.X,message.Y,message.Z);
end)


function script.drawUI()
    ui.sameLine()
    ui.pushFont(ui.Font.Title)
    if got_message then
        ui.text("ah shit" .. message_in)
    else 
        ui.text("coool")
    end
    ui.popFont()
end

local smoothFactor = 0.1 -- Controls the smoothness of the movement

function Lerp(a, b, t)
    local x = a.x + (b.x - a.x) * t
    local y = a.y + (b.y - a.y) * t
    local z = a.z + (b.z - a.z) * t
    return {x = x, y = y, z = z}
end

function script.draw3D(dt)
    render.setDepthMode(render.DepthMode.LessEqual)
    render.setCullMode(render.CullMode.None)

    local currentPosition = ball.position
    local targetPosition = ball.targetPosition

    -- Lerp between the current position and the target position using the smoothFactor
    local newPosition = Lerp(currentPosition, targetPosition, smoothFactor)

    render.debugSphere(vec3(newPosition.x, newPosition.y, newPosition.z), 1, rgbm(1,1,1,1))

    ball.position = newPosition
end