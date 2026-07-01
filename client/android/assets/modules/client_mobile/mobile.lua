local overlay
local keypad
local touchStart = 0
local updateCursorEvent
local zoomInButton
local zoomOutButton
local keypadButton
local keypadEvent
local keypadMousePos = {x=0.5, y=0.5}
local keypadTicks = 0

-- public functions
function init()
  if not g_app.isMobile() then return end
  overlay = g_ui.displayUI('mobile')
  keypad = overlay.keypad
  overlay:raise()
  
  zoomInButton = modules.client_topmenu.addLeftButton('zoomInButton', 'Zoom In', '/images/topbuttons/zoomin', function() g_app.scaleUp() end)
  zoomOutButton = modules.client_topmenu.addLeftButton('zoomOutButton', 'Zoom Out', '/images/topbuttons/zoomout', function() g_app.scaleDown() end)
  keypadButton = modules.client_topmenu.addLeftGameToggleButton('keypadButton', 'Keypad', '/images/topbuttons/keypad', function()
    keypadButton:setChecked(not keypadButton:isChecked())
    if not g_game.isOnline() then
      keypad:setVisible(false)
      return
    end
    keypad:setVisible(keypadButton:isChecked())
  end)
  keypadButton:setChecked(true)
  
  scheduleEvent(function()
    g_app.scale(5.0)
  end, 10)
  
  connect(overlay, { 
    onMousePress = onMousePress,
    onMouseRelease = onMouseRelease,
    onTouchPress = onMousePress,
    onTouchRelease = onMouseRelease,
    onMouseMove = onMouseMove 
  })
  connect(keypad, {
    onTouchPress = onKeypadTouchPress,
    onTouchRelease = onKeypadTouchRelease,  
    onMouseMove = onKeypadTouchMove
  })
  connect(g_game, { 
    onGameStart = online,
    onGameEnd = offline 
  })
  if g_game.isOnline() then
    online()
  end
end

function terminate()
  if not g_app.isMobile() then return end
  removeEvent(updateCursorEvent)
  removeEvent(keypadEvent)
  keypadEvent = nil
  disconnect(overlay, { 
    onMousePress = onMousePress,
    onMouseRelease = onMouseRelease,
    onTouchPress = onMousePress,
    onTouchRelease = onMouseRelease,
    onMouseMove = onMouseMove 
  })
  disconnect(keypad, {
    onTouchPress = onKeypadTouchPress,
    onTouchRelease = onKeypadTouchRelease,  
    onMouseMove = onKeypadTouchMove
  })
  disconnect(g_game, { 
    onGameStart = online,
    onGameEnd = offline 
  })
  zoomInButton:destroy()
  zoomOutButton:destroy()
  keypadButton:destroy()
  overlay:destroy()
  overlay = nil
end

function hide()
  overlay:hide()
end

function show()
  overlay:show()
end

function online()
  if keypadButton:isChecked() then
    keypad:raise()
    keypad:show()
  end
end

function offline()
  keypad:hide()
end

function onMouseMove(widget, pos, offset)

end

function onMousePress(widget, pos, button)
  overlay:raise()
  if button == MouseTouch then -- touch
    overlay:raise()
    overlay.cursor:show()
    overlay.cursor:setPosition({x=pos.x - 32, y = pos.y - 32})  
    touchStart = g_clock.millis()
    updateCursor()
  else
    overlay.cursor:hide()
    removeEvent(updateCursorEvent)
  end
end

function onMouseRelease(widget, pos, button)
  if button == MouseTouch then
    overlay.cursor:hide()
    removeEvent(updateCursorEvent)
  end
end

function updateCursor()
  removeEvent(updateCursorEvent)
  if not g_mouse.isPressed(MouseTouch) then return end
  local percent = 100 - math.max(0, math.min(100, (g_clock.millis() - touchStart) / 5)) -- 500 ms
  overlay.cursor:setPercent(percent)
  if percent > 0 then
    overlay.cursor:setOpacity(0.5)
    updateCursorEvent = scheduleEvent(updateCursor, 10)
  else
    overlay.cursor:setOpacity(0.8)
  end
end

function onKeypadTouchMove(widget, pos, offset)
  keypadMousePos = {x=(pos.x - widget:getPosition().x) / widget:getWidth(), 
                    y=(pos.y - widget:getPosition().y) / widget:getHeight()}
  return true
end

function onKeypadTouchPress(widget, pos, button)
  if button ~= MouseTouch then return false end
  keypadTicks = 0
  keypadMousePos = {x=(pos.x - widget:getPosition().x) / widget:getWidth(), 
                    y=(pos.y - widget:getPosition().y) / widget:getHeight()}
  executeWalk()
  return true
end

function onKeypadTouchRelease(widget, pos, button)
  if button ~= MouseTouch then return false end
  keypadMousePos = {x=(pos.x - widget:getPosition().x) / widget:getWidth(), 
                    y=(pos.y - widget:getPosition().y) / widget:getHeight()}
  executeWalk()
  removeEvent(keypadEvent)
  keypad.pointer:setMarginTop(0)
  keypad.pointer:setMarginLeft(0)
  return true
end

function executeWalk()
  removeEvent(keypadEvent)
  keypadEvent = nil
  
  if not modules.game_walking or not g_mouse.isPressed(MouseTouch) then
    keypad.pointer:setMarginTop(0)
    keypad.pointer:setMarginLeft(0)
    keypadTicks = 0
    return
  end

  -- Otimização do intervalo de agendamento para evitar lag e sobrecarga
  keypadEvent = scheduleEvent(executeWalk, 50)
  
  keypadMousePos.x = math.min(1, math.max(0, keypadMousePos.x))
  keypadMousePos.y = math.min(1, math.max(0, keypadMousePos.y))
  
  local dx = keypadMousePos.x - 0.5
  local dy = keypadMousePos.y - 0.5
  local dist = math.sqrt(dx*dx + dy*dy)
  
  -- Deadzone para evitar movimentos acidentais
  if dist < 0.05 then
    keypad.pointer:setMarginTop(0)
    keypad.pointer:setMarginLeft(0)
    return
  end

  local angle = math.atan2(dx, dy)
  local maxTop = math.abs(math.cos(angle)) * 75
  local marginTop = math.max(-maxTop, math.min(maxTop, dy * 150))
  local maxLeft = math.abs(math.sin(angle)) * 75
  local marginLeft = math.max(-maxLeft, math.min(maxLeft, dx * 150))
  
  keypad.pointer:setMarginTop(marginTop)
  keypad.pointer:setMarginLeft(marginLeft)
  
  local dir
  -- Detecção de diagonais com margem maior (0.35 para maior precisão no touch)
  if keypadMousePos.y < 0.35 and keypadMousePos.x < 0.35 then
    dir = Directions.NorthWest     
  elseif keypadMousePos.y < 0.35 and keypadMousePos.x > 0.65 then
    dir = Directions.NorthEast
  elseif keypadMousePos.y > 0.65 and keypadMousePos.x < 0.35 then
    dir = Directions.SouthWest
  elseif keypadMousePos.y > 0.65 and keypadMousePos.x > 0.65 then
    dir = Directions.SouthEast
  end
  
  if not dir then
    if math.abs(dy) > math.abs(dx) then
      dir = dy < 0 and Directions.North or Directions.South
    else
      dir = dx < 0 and Directions.West or Directions.East
    end  
  end
  
  if dir then
    -- Aumenta a velocidade de repetição após o primeiro passo
    modules.game_walking.walk(dir, keypadTicks)
    if keypadTicks == 0 then
      keypadTicks = 1 -- Valor inicial pequeno para o primeiro passo
    else
      keypadTicks = g_clock.millis() -- Usa o tempo atual para sincronizar com a engine de walk
    end
  end
end

local hotkeysWindow

function setupHotkeys()
  if hotkeysWindow then return end
  hotkeysWindow = g_ui.displayUI('hotkeys')
  
  local h1 = hotkeysWindow:getChildById('hotkey1')
  h1.onClick = function() g_game.talk('exura') end -- Exemplo, pode ser mapeado para as hotkeys reais do jogo

  local h2 = hotkeysWindow:getChildById('hotkey2')
  h2.onClick = function() g_game.talk('exori vis') end

  local h3 = hotkeysWindow:getChildById('hotkey3')
  h3.onClick = function() modules.game_hotkeys.useHotkey(1) end
end

-- Adicionar chamada no init() original se necessário ou via hook
