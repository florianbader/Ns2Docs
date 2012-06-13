function table.copy(t)
	local copy = {}
	for key, value in pairs(t) do
		copy[key] = value
	end
	return copy
end


function table.combine(tbl, otherTbl, errFunc)
    local ret = {}
    
    for key, value in pairs(tbl) do
        ret[key] = value
    end
    if otherTbl then
		for key, value in pairs(otherTbl) do
			if ret[key] == nil then
				ret[key] = value
			elseif errFunc then
				local corrected = errFunc(tbl, otherTbl, ret, key, ret[value], value)
				if type(corrected) == "table" then
					ret = corrected
				else
					break
				end
			end
		end
	end
    return ret
end

function string.split(str, delim, maxNb)
    
    -- Eliminate bad cases...
    if string.find(str, delim) == nil then
        return { str }
    end
    if maxNb == nil or maxNb < 1 then
        maxNb = 0    -- No limit
    end
    local result = {}
    local pat = "(.-)" .. delim .. "()"
    local nb = 0
    local lastPos
    for part, pos in string.gfind(str, pat) do
        nb = nb + 1
        result[nb] = part
        lastPos = pos
        if nb == maxNb then break end
    end
    -- Handle the last field
    if nb ~= maxNb then
        result[nb + 1] = string.sub(str, lastPos)
    end
    return result
end

function TableToString(tbl, indent)
	if not tbl then
		return "nil table"
	end
	
	if not indent then
		indent = 0
	end
	
	local str = ""
	for key, value in pairs(tbl) do
		for i=1, indent do
			str = str .. "-"
		end
		str = str .. key .. ": " .. tostring(value) .. "\n"
		if type(value) == "table" then
			str = str .. TableToString(value, indent + 4)
		end
	end
	return str
end

function string.endswith(str, endingWith)
	return #str >= #endingWith and str:find(endingWith, #str-#endingWith+1, true) and true or false
end