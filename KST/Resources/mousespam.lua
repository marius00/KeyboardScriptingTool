-- Mouse spam library for KST
-- It is not considered safe to edit this file.
-- This file will be overwritten at will.
--
-- Usage:
-- Call ms_add('G3', 'G3', 'LMB') to toggle spamming the left mouse button with G3
-- Or if using G-hub with G3 mapped to F3, then ms_add('F3', 'G3', 'LMB')
-- Pass a fourth argument to click only while the key is held: ms_add('F3', 'G3', 'LMB', true)
--   (hold mode requires a real keyboard key, G-keys do not send KeyUpEvents)
-- Mouse buttons: LMB, RMB, MMB
-- In OnEvent call ms_OnEvent
-- (Optional) In output text call ms_outputHelpText()
-- (Optional) In reset call ms_reset()

-- ms_setup[trigger] = { button = 'LMB', color = 'G3', hold = false }
ms_setup = {}
ms_activeTriggers = {}

-- Resets/stops all mouse spam.
function ms_reset()
	for trigger, cfg in pairs(ms_setup) do
		SetBacklightColor(cfg.color, 100, 0, 0)
	end

	ms_activeTriggers = {}
end

function ms_add(keyToTrigger, keyToColor, buttonToSpam, holdToSpam)
	ms_setup[keyToTrigger] = {
		button = buttonToSpam,
		color = keyToColor,
		hold = holdToSpam == true
	}

	SetBacklightColor(keyToColor, 100, 0, 0)
	OutputLogMessage('Configured key "{0}" to spam mouse button "{1}"', keyToTrigger, buttonToSpam)
end

function ms_outputHelpText()
	for trigger, cfg in pairs(ms_setup) do
		if cfg.hold then
			OutputLogMessage('- {0}: Hold to spam {1}', cfg.color, cfg.button)
		else
			OutputLogMessage('- {0}: Spam {1}', cfg.color, cfg.button)
		end
	end
end

function ms_setActive(trigger, active)
	local cfg = ms_setup[trigger]
	if cfg == nil then
		return
	end

	if active then
		ms_activeTriggers[trigger] = true
		OutputLogMessage('Starting to spam {0}', cfg.button)
		SetBacklightColor(cfg.color, 0, 100, 0)
	else
		ms_activeTriggers[trigger] = nil
		OutputLogMessage('Stopped spamming {0}', cfg.button)
		SetBacklightColor(cfg.color, 100, 0, 0)
	end
end

function ms_OnEvent(event, arg, modifiers)
	if event == TickEvent then
		for trigger, _ in pairs(ms_activeTriggers) do
			MouseClick(ms_setup[trigger].button)
		end
	elseif event == KeyDownEvent then
		local cfg = ms_setup[arg]
		if cfg ~= nil then
			if cfg.hold then
				if ms_activeTriggers[arg] == nil then
					ms_setActive(arg, true)
				end
			else
				ms_setActive(arg, ms_activeTriggers[arg] == nil)
			end
		end
	elseif event == KeyUpEvent then
		local cfg = ms_setup[arg]
		if cfg ~= nil and cfg.hold and ms_activeTriggers[arg] ~= nil then
			ms_setActive(arg, false)
		end
	end
end
