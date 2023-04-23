Ball = {}
Ball.__index = Ball

function Ball.new(position, velocity)
    local self = setmetatable({}, Ball)
    self.position = position
    self.velocity = velocity
    self.rotationQuaternion = quat(0, 0, 0, 1) -- Identity quaternion
    return self
end


function Ball:update(dt)
    self.position = self.position + self.velocity * dt
    local rotationAxis, rotationAngle = self:getRotation(dt)
    local rotationAngleRad = math.rad(-rotationAngle) -- convert to radians and negate the angle
    local deltaRotation = quat_fromAxisAngle(rotationAxis, rotationAngleRad)
    self.rotationQuaternion = quat_multiply(deltaRotation, self.rotationQuaternion)
end

function quat_fromAxisAngle(axis, angle)
    local half_angle = angle * 0.5
    local sin_half_angle = math.sin(half_angle)
    local cos_half_angle = math.cos(half_angle)
    local x = axis.x * sin_half_angle
    local y = axis.y * sin_half_angle
    local z = axis.z * sin_half_angle
    local w = cos_half_angle
    return quat(x, y, z, w)
end


function quat_multiply(a, b)
    local w = a.w * b.w - a.x * b.x - a.y * b.y - a.z * b.z
    local x = a.w * b.x + a.x * b.w + a.y * b.z - a.z * b.y
    local y = a.w * b.y - a.x * b.z + a.y * b.w + a.z * b.x
    local z = a.w * b.z + a.x * b.y - a.y * b.x + a.z * b.w
    return quat(x, y, z, w)
end

function rotate_vector_by_quat(vector, q)
    local q_conj = quat(-q.x, -q.y, -q.z, q.w)
    local q_vec = quat(vector.x, vector.y, vector.z, 0)
    local temp = quat_multiply(q, q_vec)
    local rotated_vector_quat = quat_multiply(temp, q_conj)
    return vec3(rotated_vector_quat.x, rotated_vector_quat.y, rotated_vector_quat.z)
end


function normalized(vec)
    local length = vec:length()
    return vec3(vec.x / length, vec.y / length, vec.z / length)
end


function Ball:getRotation(dt)
    local up = vec3(0, 1, 0)
    local horizontalVelocity = vec3(self.velocity.x, 0, self.velocity.z)
    
    if horizontalVelocity.x == 0 and horizontalVelocity.z == 0 then
        return up, 0
    end

    local normalizedHorizontalVelocity = normalized(horizontalVelocity)
    local rotationAxis = vec3_cross(normalizedHorizontalVelocity, up)
    local distance = horizontalVelocity:length() * dt
    local radius = 1 -- assuming the sphere's radius is 1
    local rotationAngle = math.deg(distance / radius) -- converting radians to degrees

    return rotationAxis, rotationAngle
end


--Start the ball under the map before we get the server position...
local ballPosition = vec3(0,-10,0)
local ballVelocity = vec3(0,-10,0)

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

function vec3_dot(a, b)
    return a.x * b.x + a.y * b.y + a.z * b.z
end

function vec3_cross(a, b)
    return vec3(
        a.y * b.z - a.z * b.y,
        a.z * b.x - a.x * b.z,
        a.x * b.y - a.y * b.x
    )
end

local rotationAxis, rotationAngle
local rotationAngleRad


function rotatePointAroundAxis(point, axis, angle)
    local cosAngle = math.cos(angle)
    local sinAngle = math.sin(angle)
    local dotProduct = vec3_dot(point, axis)
    local crossProduct = vec3_cross(point, axis)

    local rotatedPoint = vec3(
        point.x * cosAngle + crossProduct.x * sinAngle + axis.x * dotProduct * (1 - cosAngle),
        point.y * cosAngle + crossProduct.y * sinAngle + axis.y * dotProduct * (1 - cosAngle),
        point.z * cosAngle + crossProduct.z * sinAngle + axis.z * dotProduct * (1 - cosAngle)
    )

    return rotatedPoint
end



function script.update(dt)
    ball:update(dt)
end

function DrawSphere(center, radius, latSegments, longSegments, rotationQuaternion)

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

        local v00 = rotate_vector_by_quat(vec3(x00, y0, z00), rotationQuaternion) + center
        local v01 = rotate_vector_by_quat(vec3(x01, y0, z01), rotationQuaternion) + center
        local v10 = rotate_vector_by_quat(vec3(x10, y1, z10), rotationQuaternion) + center
        local v11 = rotate_vector_by_quat(vec3(x11, y1, z11), rotationQuaternion) + center
        

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

    DrawSphere(ball.position, 1, 8, 8, ball.rotationQuaternion)
end