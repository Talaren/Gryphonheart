


function Linq(lt)
	lt = lt or {};

	lt.Where = function(f)
		local t = Linq();
		for i=1,#(lt) do
			if f(lt[i]) then
				table.insert(t,lt[i]);
			end
		end
		return t;
	end

	lt.Select = function(f)
		local t = Linq();
		for i=1,#(lt) do
			table.insert(t,f(lt[i]));
		end
		return t;
	end

	lt.Foreach = function(f)
		for i=1,#(lt) do
			f(lt[i]);
		end
	end

	lt.Count = function()
		return #(lt);
	end

	lt.Any = function()
		return #(lt) > 0;
	end

	lt.First = function()
		return lt[1];
	end

	lt.MaxBy = function(f)
		local maxIndex = 1;
		local maxValue;
		for i=1,#(lt) do
			local val = f(lt[i]);
			if not(maxValue) or val > maxValue then
				maxIndex = i;
				maxValue = val;
			end
		end
		return lt[maxIndex];
	end

	lt.MinBy = function(f)
		local minIndex = 0;
		local minValue = 0;
		for i=1,#(lt) do
			local val = f(lt[i]);
			if val < minValue then
				minIndex = i;
				minValue = val;
			end
		end
		return lt[minIndex];
	end

	lt.Sum = function(f)
		local sum = 0;
		for i=1,#(lt) do
			sum = sum + f(lt[i]);
		end
		return sum;
	end

	lt.Contains = function(v)
		for i=1,#(lt) do
			if v == lt[i] then
				return true;
			end
		end
		return false;
	end

	lt.None = function()
		return #(lt) == 0;
	end

	lt.Union = function(t)
		local res = Linq();
		lt.Foreach(function(v) table.insert(res, v); end)
		t.Foreach(function(v) if not(lt.Contains(v)) then table.insert(res, v); end end)
		return res;
	end

	lt.Intersection = function(t)
		local res = Linq();
		t.Foreach(function(v) if lt.Contains(v) then table.insert(res, v); end end)
		return res;
	end

	lt._linq = true;

	for i=1,#(lt) do
		local t = lt[i];
		if type(t) == "table" and not(t._linq) then
			lt[i] = Linq(t);
		end
	end

	return setmetatable(lt, {
		--[[__newindex = function(t, key, value)
			if type(key) == "number" and type(value) == "table" then
				lt[key] = Linq(value);
			end
		end  --]]
	});
end

