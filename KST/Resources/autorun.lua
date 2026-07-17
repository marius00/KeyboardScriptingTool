-- Autorun library for KST
-- It is not considered safe to edit this file.
-- This file will be overwritten at will.
--
-- Toggles holding down the movement key (typically W).
-- Autorun is cancelled by pressing the trigger again, by releasing the run key,
-- or by pressing any of the cancel keys (typically S).
--
-- Usage:
-- Call ar_add('G3', 'G3', 'W', {'S'}) to make G3 toggle autorun on W, cancelled by W or S
-- Or if using G-hub with G3 mapped to F3, then ar_add('F3', 'G3', 'W', {'S'})
-- In OnEvent call ar_OnEvent
-- (Optional) In output text call ar_outputHelpText()
-- (Optional) In reset call ar_reset()

ar_trigger = nil
ar_color = nil
ar_runKey = 'W'
ar_cancelKeys = {}
ar_active = false

function ar_add(keyToTrigger, keyToColor, runKey, cancelKeys)
	ar_trigger = keyToTrigger
	ar_color = keyToColor
	ar_runKey = runKey
	ar_cancelKeys = cancelKeys or {}
	ar_active = false

	SetBacklightColor(keyToColor, 100, 0, 0)
	OutputLogMessage('Configured key "{0}" to autorun using "{1}"', keyToTrigger, runKey)
end

function ar_outputHelpText()
	if ar_trigger ~= nil then
		OutputLogMessage('- {0}: Autorun ({1} or {0} to cancel it)', ar_color, ar_runKey)
	end
end

function ar_reset()
	ar_stop()
end

function ar_stop()
	if ar_trigger == nil then
		return
	end

	-- Always release the key, in case the script was reloaded while running
	ar_active = false
	SetBacklightColor(ar_color, 100, 0, 0)
	KeyUp(ar_runKey)
end

function ar_start()
	ar_active = true
	SetBacklightColor(ar_color, 0, 100, 0)
	KeyDown(ar_runKey)
	OutputLogMessage('Autorun enabled')
end

function ar_isCancelKey(arg)
	for _, key in pairs(ar_cancelKeys) do
		if key == arg then
			return true
		end
	end
	return false
end

function ar_OnEvent(event, arg, modifiers)
	if ar_trigger == nil then
		return
	end

	if event == KeyDownEvent then
		if arg == ar_trigger then
			if ar_active then
				OutputLogMessage('Cancelling autorun')
				ar_stop()
			else
				ar_start()
			end
		elseif ar_active and ar_isCancelKey(arg) then
			OutputLogMessage('Cancelling autorun due to {0}', arg)
			ar_stop()
		end
	elseif event == KeyUpEvent then
		-- Key "UP" event, because the run key is being held down and we'll constantly receive it.
		if ar_active and arg == ar_runKey then
			OutputLogMessage('Cancelling autorun due to {0}', arg)
			ar_stop()
		end
	end
end
