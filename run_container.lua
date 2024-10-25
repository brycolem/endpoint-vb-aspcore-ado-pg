#!/usr/bin/env lua

-- Function to get environment variables or exit if not set
local function get_env_var(name)
    local value = os.getenv(name)
    if not value or value == "" then
        io.stderr:write("Error: Environment variable " .. name .. " is not set.\n")
        os.exit(1)
    end
    return value
end

-- Get environment variables
local DATABASE = get_env_var("DATABASE")
local DB_USER = get_env_var("DB_USER")
local DB_PWD = get_env_var("DB_PWD")

-- Build the container image
local build_command = "podman build -t vb-aspnetcore-ado:latest ."
print("Building the container image...")
local build_success, build_reason, build_code = os.execute(build_command)
if not build_success then
    io.stderr:write("Error: Failed to build the container image.\n")
    io.stderr:write("Reason: " .. tostring(build_reason) .. "\n")
    os.exit(1)
end

-- Run the container
local run_command = string.format(
    "podman run -d -p 8001:8001 --network=host " ..
    "--ulimit nofile=65536:65536 -e DATABASE=%q -e DB_USER=%q -e DB_PWD=%q " ..
    "--replace --name vb-aspnetcore-ado vb-aspnetcore-ado:latest",
    DATABASE, DB_USER, DB_PWD
)

print("Running the container...")
local run_success, run_reason, run_code = os.execute(run_command)
if not run_success then
    io.stderr:write("Error: Failed to run the container.\n")
    io.stderr:write("Reason: " .. tostring(run_reason) .. "\n")
    os.exit(1)
end

print("Container is running successfully.")
