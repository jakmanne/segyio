project(dotnet)

if(BUILD_DOTNET)
set (msbuild "msbuild")
set(dotnet_sln "dotnet.sln")

message("Does this print") 

add_custom_target(
${dotnet_sln} ALL
COMMAND ${msbuild}
message("Does enter add custom target")
VERBATIM
)

message("Finished")
endif()
