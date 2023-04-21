local ball = {
    position = vec3(0, 0, 0),
    targetPosition = vec3(0, 0, 0)
}

local got_message = false;

local message_in = nil;

local function normalize(v)
    local length = math.sqrt(v.x * v.x + v.y * v.y + v.z * v.z)
    return vec3(v.x / length, v.y / length, v.z / length)
end

local function dot(v1, v2)
    return v1.x * v2.x + v1.y * v2.y + v2.z * v2.z
end

local assettoBallEvent = ac.OnlineEvent({
    ac.StructItem.key("assettoBallPosition"),
    X = ac.StructItem.float(),
    Y = ac.StructItem.float(),
    Z = ac.StructItem.float()
}, function(sender, message)
    if sender ~= nil then return end
    got_message = true;
    ball.targetPosition = vec3(message.X,message.Y,message.Z);
end)

local function is_ball_visible_and_angle(ballPos)
    -- Get camera properties
    local camPos = ac.getCameraPosition()
    local camDir = ac.getCameraForward()
    local camFOV = ac.getCameraFOV()
  
    -- Calculate the vector from the camera position to the ball position
    local vecToBall = ballPos:sub(camPos)
  
    -- Normalize camera direction and vector to ball
    local camDirNormalized = normalize(camDir)
    local vecToBallNormalized = normalize(vecToBall)
  
    -- Calculate dot product and angle between camera direction and vector to ball
    local dotProduct = dot(vecToBallNormalized, camDirNormalized)
    local angleToBall = math.acos(dotProduct) * (180 / math.pi) -- Convert radians to degrees
  
    -- Check if the angle is within the camera's FOV
    local isVisible = angleToBall <= (camFOV / 2)
  
    return isVisible, angleToBall
  end
  


function script.drawUI()
    ui.sameLine()
    ui.pushFont(ui.Font.Title)
    if got_message then
        --local isVisible, angleToBall = is_ball_visible_and_angle(ball.targetPosition)
        --ui.text("is ball visible? " .. tostring(isVisible) .. "angle to ball: ?" .. tostring(angleToBall))
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

function DrawSphere(center, radius, latSegments, longSegments)

  
    -- Calculate the delta angle for latitude and longitude
    local latStep = math.pi / latSegments
    local longStep = 2 * math.pi / longSegments
  
    for i = 0, latSegments - 1 do
      local lat0 = i * latStep
      local lat1 = (i + 1) * latStep
  
      local y0 = radius * math.cos(lat0)
      local y1 = radius * math.cos(lat1)
  
      local r0 = radius * math.sin(lat0)
      local r1 = radius * math.sin(lat1)
  
      for j = 0, longSegments - 1 do
        local long0 = j * longStep
        local long1 = (j + 1) * longStep
  
        local x00 = r0 * math.cos(long0)
        local z00 = r0 * math.sin(long0)
  
        local x01 = r0 * math.cos(long1)
        local z01 = r0 * math.sin(long1)
  
        local x10 = r1 * math.cos(long0)
        local z10 = r1 * math.sin(long0)
  
        local x11 = r1 * math.cos(long1)
        local z11 = r1 * math.sin(long1)
  
        local v00 = vec3(center.x + x00, center.y + y0, center.z + z00)
        local v01 = vec3(center.x + x01, center.y + y0, center.z + z01)
        local v10 = vec3(center.x + x10, center.y + y1, center.z + z10)
        local v11 = vec3(center.x + x11, center.y + y1, center.z + z11)
  
        local uv00 = vec2(i / latSegments, j / longSegments)
        local uv01 = vec2(i / latSegments, (j + 1) / longSegments)
        local uv10 = vec2((i + 1) / latSegments, j / longSegments)
        local uv11 = vec2((i + 1) / latSegments, (j + 1) / longSegments)
  
        -- Draw first triangle
        render.glBegin(render.GLPrimitiveType.Triangles)
        render.glSetColor(rgbm(1,1,1,1))
        render.glVertex(v00)
        render.glVertex(v10)
        render.glVertex(v01)
        render.glEnd()
  
        -- Draw second triangle
        render.glBegin(render.GLPrimitiveType.Triangles)
        render.glSetColor(rgbm(1,1,1,1))
        render.glVertex(v01)
        render.glVertex(v10)
        render.glVertex(v11)
        render.glEnd()
      end
    end
end
  

function script.draw3D(dt)
    render.setDepthMode(render.DepthMode.LessEqual)
    render.setCullMode(render.CullMode.None)

    local currentPosition = ball.position
    local targetPosition = ball.targetPosition

    -- Lerp between the current position and the target position using the smoothFactor
    local newPosition = Lerp(currentPosition, targetPosition, smoothFactor)

    DrawSphere(newPosition, 1, 8, 8)
    --render.debugSphere(newPosition, 1, rgbm(1,1,1,1))

    ball.position = newPosition
end