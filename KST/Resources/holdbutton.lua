-- Hold button library for KST
-- It is not considered safe to edit this file.
-- This file will be overwritten at will.
--
-- Toggles holding down a mouse button or a keyboard key.
-- Press once to hold it down, press again to release it.
--
-- Usage:
-- Call hb_add('G5', 'G5', 'LMB') to hold down the left mouse button with G5
-- Or if using G-hub with G5 mapped to F5, then hb_add('F5', 'G5', 'LMB')
-- Mouse buttons: LMB, RMB, MMB. Anything else is treated as a keyboard key.
-- In OnEvent call hb_OnEvent
-- (Optional) In output text call hb_outputHelpText()
-- (Optional) In reset call hb_reset()

-- hb_setup[trigger] = { button = 'LMB', color = 'G5', isMouse = true }
hb_setup = {}
hb_activeTriggers = {}

function hb_isMouseButton(button)
	return button == 'LMB' or button == 'RMB' or button == 'MMB'
end

-- Releases everything currently being held down.
function hb_reset()
	for trigger, cfg in pairs(hb_setup) do
		if hb_activeTriggers[trigger] ~= nil then
			hb_release(trigger)
		end
		SetBacklightColor(cfg.color, 100, 0, 0)
	end

	hb_activeTriggers = {}
end

function hb_add(keyToTrigger, keyToColor, buttonToHold)
	hb_setup[keyToTrigger] = {
		button = buttonToHold,
		color = keyToColor,
		isMouse = hb_isMouseButton(buttonToHold)
	}

	SetBacklightColor(keyToColor, 100, 0, 0)
	OutputLogMessage('Configured key "{0}" to hold down "{1}"', keyToTrigger, buttonToHold)
end

function hb_outputHelpText()
	for trigger, cfg in pairs(hb_setup) do
		OutputLogMessage('- {0}: Hold down {1}', cfg.color, cfg.button)
	end
end

function hb_press(trigger)
	local cfg = hb_setup[trigger]
	hb_activeTriggers[trigger] = true

	if cfg.isMouse then
		MouseDown(cfg.button)
	else
		KeyDown(cfg.button)
	end

	OutputLogMessage('Holding down {0}', cfg.button)
	SetBacklightColor(cfg.color, 0, 100, 0)
end

function hb_release(trigger)
	local cfg = hb_setup[trigger]
	hb_activeTriggers[trigger] = nil

	if cfg.isMouse then
		MouseUp(cfg.button)
	else
		KeyUp(cfg.button)
	end

	OutputLogMessage('Released {0}', cfg.button)
	SetBacklightColor(cfg.color, 100, 0, 0)
end

-- Called when a mouse button we're holding is pressed manually.
-- The game will have seen the button go up (a physical click is down+up), so it has already
-- cancelled the hold. We clear our own state to match, without sending another MouseUp,
-- so that a single trigger press re-enables the hold instead of needing two.
function hb_cancelHeldButton(button)
	for trigger, cfg in pairs(hb_setup) do
		if cfg.isMouse and cfg.button == button and hb_activeTriggers[trigger] ~= nil then
			hb_activeTriggers[trigger] = nil
			OutputLogMessage('{0} was clicked manually, cancelling the hold', button)
			SetBacklightColor(cfg.color, 100, 0, 0)
		end
	end
end

function hb_OnEvent(event, arg, modifiers)
	if event == KeyDownEvent then
		if hb_setup[arg] ~= nil then
			-- A trigger key was pressed, toggle the hold.
			if hb_activeTriggers[arg] == nil then
				hb_press(arg)
			else
				hb_release(arg)
			end
		else
			-- Might be a manual click of a button we are currently holding.
			hb_cancelHeldButton(arg)
		end
	end
end
