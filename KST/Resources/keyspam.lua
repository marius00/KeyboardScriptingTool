-- Key spam library for KST
-- It is not considered safe to edit this file.
-- This file will be overwritten at will.
--
-- Usage:
-- Call ks_add('G5', 'G5', 'E') to toggle spamming E with G5
-- Or if using G-hub with G5 mapped to F5, then ks_add('F5', 'G5', 'E')
-- Pass a fourth argument to spam only while the key is held: ks_add('F5', 'G5', 'E', true)
--   (hold mode requires a real keyboard key, G-keys do not send KeyUpEvents)
-- In OnEvent call ks_OnEvent
-- (Optional) In output text call ks_outputHelpText()
-- (Optional) In reset call ks_reset()

-- ks_setup[trigger] = { key = 'E', color = 'G5', hold = false }
ks_setup = {}
ks_activeTriggers = {}

-- Resets/stops all key spam.
function ks_reset()
	for trigger, cfg in pairs(ks_setup) do
		SetBacklightColor(cfg.color, 100, 0, 0)
	end

	-- No active keys
	ks_activeTriggers = {}
end

-- Call this to setup the keymapping, ex: ks_add('G4', 'G4', 'E') will make G4 toggle "spam E"
function ks_add(keyToTrigger, keyToColor, keyToSpam, holdToSpam)
	-- With some logitech setups (specifically new g-hub), the G1 key will be received as F1
	-- The 'color' field lets us color G1 while listening for F1
	ks_setup[keyToTrigger] = {
		key = keyToSpam,
		color = keyToColor,
		hold = holdToSpam == true
	}

	SetBacklightColor(keyToColor, 100, 0, 0)
	OutputLogMessage('Configured key "{0}" to spam key "{1}"', keyToTrigger, keyToSpam)
end

function ks_outputHelpText()
	for trigger, cfg in pairs(ks_setup) do
		if cfg.hold then
			OutputLogMessage('- {0}: Hold to spam the "{1}" key', cfg.color, cfg.key)
		else
			OutputLogMessage('- {0}: Spam the "{1}" key', cfg.color, cfg.key)
		end
	end
end

function ks_setActive(trigger, active)
	local cfg = ks_setup[trigger]
	if cfg == nil then
		return
	end

	if active then
		ks_activeTriggers[trigger] = true
		OutputLogMessage('Starting to spam key "{0}"', cfg.key)
		SetBacklightColor(cfg.color, 0, 100, 0)
	else
		ks_activeTriggers[trigger] = nil
		OutputLogMessage('Stopped spamming key "{0}"', cfg.key)
		SetBacklightColor(cfg.color, 100, 0, 0)
	end
end

function ks_OnEvent(event, arg, modifiers)
	if event == TickEvent then
		-- Spam the active keys
		for trigger, _ in pairs(ks_activeTriggers) do
			KeyPress(ks_setup[trigger].key)
		end
	elseif event == KeyDownEvent then
		local cfg = ks_setup[arg]
		if cfg ~= nil then
			if cfg.hold then
				-- Hold mode: active for as long as the key is held down
				if ks_activeTriggers[arg] == nil then
					ks_setActive(arg, true)
				end
			else
				-- Toggle mode
				ks_setActive(arg, ks_activeTriggers[arg] == nil)
			end
		end
	elseif event == KeyUpEvent then
		local cfg = ks_setup[arg]
		if cfg ~= nil and cfg.hold and ks_activeTriggers[arg] ~= nil then
			ks_setActive(arg, false)
		end
	end
end
