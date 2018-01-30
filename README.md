# Xlua_Notes
腾讯车前辈的开源热更框架 地址https://github.com/Tencent/xLua ，因为只是增加了案例注释和在原案例基础上扩展，就不打扰原作者了。有需要的萌新可以参照注释和这个文档，更好的学习xlua。<br>

近期更新计划：热更完整案例。<br>
如果存在版本问题报错，请reimport

1.热更协程写法<br>
停一帧 waitforendofframe,yeild return 0; coroutine.yield(0)
<img src="https://i.loli.net/2018/01/19/5a6164e7e8143.png" alt="QQ图片20180119112352.png" title="QQ图片20180119112352.png" />
<img src="https://i.loli.net/2018/01/19/5a616410884c0.png" alt="QQ图片20180119111913.png" title="QQ图片20180119111913.png" />
<img src="https://i.loli.net/2018/01/19/5a616410d3921.png" alt="QQ图片20180119112002.png" title="QQ图片20180119112002.png" />
<img src="https://i.loli.net/2018/01/19/5a6164174271c.jpg" alt="QQ图片20180119112008.jpg" title="QQ图片20180119112008.jpg" />

    function CO.yield_return(yield_to)
    if __fci_client then
        -- 改造过的实现
        local co = coroutine.running() or error ('this function must be run in coroutine')

        -- 设置C#回调以便让C#帮助自动恢复协程
        local function func_for_resuming()
            CO.call(co)
        end

        local runner = co_runner[co]
        if runner == nil then
            if default_runner == nil then
                default_runner = CS.LuaManager.Instance:GetComponent(typeof(CS.CoroutineRunner))
            end
            runner = default_runner
        end

        runner:YieldAndCallback(yield_to, func_for_resuming)

        -- （后置）挂起当前协程
        coroutine.yield()
    end
    end

2.Unity下XLua方案的各值类型GC优化深度剖析
http://www.gameres.com/700911.html

