Ball = {}
Ball.__index = Ball

function Ball.new(position, velocity)
    local self = setmetatable({}, Ball)
    self.position = position
    self.velocity = velocity
    return self
end

function Ball:update(dt)
    self.position = self.position + self.velocity * dt
end

--Start the ball under the map before we get the server position...
local ballPosition = vec3(0,-10,0)
local ballVelocity = vec3(0,0,0)

local ball = Ball.new(ballPosition, ballVelocity)

function onServerUpdate(recievedPosition, recievedVelocity)
    local positionError = recievedPosition - ball.position
    local velocityError = recievedVelocity - ball.velocity

    local positionThreshold = 0.1
    local velocityThreshold = 0.1

    local lerpFactor = 0.1

    if positionError:length() > positionThreshold then
        ball.position = vec3Lerp(ball.position, recievedPosition, lerpFactor)
    end

    if velocityError:length() > velocityThreshold then
        ball.velocity = vec3Lerp(ball.velocity, recievedVelocity, lerpFactor)
    end
end

function vec3Lerp(a, b, t)
    local x = a.x + (b.x - a.x) * t
    local y = a.y + (b.y - a.y) * t
    local z = a.z + (b.z - a.z) * t
    return vec3(x, y, z)
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
    onServerUpdate(vec3(message.PosX,message.PosY,message.PosZ), vec3(message.VelX,message.VelY,message.VelZ))
end)

function script.update(dt)
    ball:update(dt)
end

function DrawSphere(center, radius, latSegments, longSegments)

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

        -- Determine the color based on the sum of i and j
        local color
        if (i + j) % 2 == 0 then
          color = rgbm(1, 1, 1, 1) -- White
        else
          color = rgbm(0, 0, 0, 1) -- Black
        end

        -- Draw first triangle
        render.glBegin(render.GLPrimitiveType.Triangles)
        render.glSetColor(color)
        render.glVertex(v00)
        render.glVertex(v10)
        render.glVertex(v01)
        render.glEnd()
  
        -- Draw second triangle
        render.glBegin(render.GLPrimitiveType.Triangles)
        render.glSetColor(color)
        render.glVertex(v01)
        render.glVertex(v10)
        render.glVertex(v11)
        render.glEnd()
      end
    end
end

  

function script.draw3D(dt)
    render.setDepthMode(render.DepthMode.LessEqual)
    render.setCullMode(render.CullMode.ShadowsDouble)
    DrawSphere(ball.position, 1, 8, 8)
end