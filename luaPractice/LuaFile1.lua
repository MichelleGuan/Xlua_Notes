--region *.lua
--Date
--此文件由[BabeLua]插件自动生成

 xlua.hotfix(CS.StatefullTest, {
                ['.ctor'] = function(csobj)
                    return {evt = {}, start = 0}
                end;
                
                set_AProp = function(self, v)
                    print('set_AProp', v)
                    self.AProp = v
                end;
                get_AProp = function(self)
                    return self.AProp
                end;
                get_Item = function(self, k)
                    print('get_Item', k)
                    return 1024
                end;
                set_Item = function(self, k, v)
                    print('set_Item', k, v)
                end;
                add_AEvent = function(self, cb)
                    print('add_AEvent', cb)
                    table.insert(self.evt, cb)
                end;
                remove_AEvent = function(self, cb)
                   print('remove_AEvent', cb)
                   for i, v in ipairs(self.evt) do
                       if v == cb then
                           table.remove(self.evt, i)
                           break
                       end
                   end
                end;
                Start = function(self)
                    print('Start')
                    for _, cb in ipairs(self.evt) do
                        cb(self.start, 2)
                    end
                    self.start = self.start + 1
                end;
                StaticFunc = function(a, b, c)
                   print(a, b, c)
                end;
                GenericTest = function(self, a)
                   print(self, a)
                end;
                Finalize = function(self)
                   print('Finalize', self)
                end
           })

--endregion
