local SparkObjectHandler = require "SparkObjectHandler"
local Table = Ns2Docs.Spark.Table

local ClassHandler = {}

function ClassHandler.Capture(offset, name, inherits, offsetEnd)
	if not offsetEnd then
		offsetEnd = inherits
		inherits = nil
	end
	return 
	{
		__name = 'Class',
		name = name, 
		inherits = inherits,
		offset = offset,
		offsetEnd = offsetEnd,
		Create = ClassHandler.Create
	}
end

function ClassHandler.Create(game, source, info, extraData)
	local name = info.name
	local addClassToGame = false
	
	local sparkClass = game:FindTableWithName(name)
	if sparkClass == nil then
		sparkClass = Table(name)
		addClassToGame = true
	end
	sparkClass:set_CanInstantiate(true)
	
	if info.inherits then
		local inherits = game:FindTableWithName(info.inherits)
		if inherits == nil then
			inherits = Table(info.inherits)
			inherits.CanInstantiate = true
			game:get_Tables():Add(inherits)
		end
		sparkClass.BaseTable = inherits
	end

	SparkObjectHandler.Init(game, source, sparkClass, info, extraData)
	
	if addClassToGame then
		game:get_Tables():Add(sparkClass)
	end
	
	return sparkClass
end

return ClassHandler