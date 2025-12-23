#!/bin/bash

# Add Watchman trigger for dotnet watch
watchman watch .

watchman -j <<- 'EOF'
["trigger", ".", {
  "name": "dotnet-watch",
  "command": ["dotnet", "run"],
  "expression": ["type", "f"]
  ],
}]
EOF

echo "Watchman is set up for hot reload!"
