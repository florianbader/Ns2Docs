local SparkObjectHandler = require 'SparkObjectHandler'
local Table = Ns2Docs.Spark.Table

local PrepareMixinHandler = {}

function PrepareMixinHandler.Capture(offset, className, mixinName)
	return 
	{
		__name = 'PrepareMixin',
		className = className, 
		mixinName = mixinName,
		offset = offset,
		Create = PrepareMixinHandler.Create
	}
end

function PrepareMixinHandler.Create(game, source, info, extraData)
	local className = info.className
	local mixinName = info.mixinName
	
	local sparkClass = game:FindTableWithName(className)
	if not sparkClass then
		sparkClass = Table(className)
		game:get_Tables():Add(sparkClass)
	end
	
	local mixin = game:FindTableWithName(mixinName)
	if not mixin then
		mixin = Table(mixinName)
		game:get_Tables():Add(mixin)
	end
	sparkClass:get_Mixins():Add(mixin)
end

return PrepareMixinHandler