#!/bin/bash

# File extensions
SCRIPT_EXT="\.cs$|\.meta$|\.asset$"
UNITY_ASSET_EXT="\.prefab$|\.unity$|\.asset$|\.meta$|\.asset$"
MODEL_EXT="\.fbx$|\.obj$|\.3ds$|\.meta$|\.asset$"
TEXTURE_EXT="\.png$|\.jpg$|\.jpeg$|\.tga$|\.psd$|\.meta$|\.asset$"
MATERIAL_EXT="\.mat$|\.meta$|\.asset$"
TERRAIN_EXT="\.asset$|\.meta$|\.asset$"
SCENE_EXT="\.unity$|\.meta$|\.asset$"

check_directory() {

    local directory=$1
    local allowed_extensions=$2
    local description=$3

    # Check if the directory exists
    if [ ! -d "$directory" ]; then
        echo "WARNING: Directory $directory does not exist"
        return
    fi

    echo "Checking $description..."
    # Find invalid files, excluding folder-associated .meta files
    invalid_files=$(find "$directory" -type f | grep -vE "$allowed_extensions")

    if [ ! -z "$invalid_files" ]; then
        echo "ERROR: Disallowed files in $description:"
        echo "$invalid_files"
        echo "Allowed extensions are: $allowed_extensions"
        exit 1
    fi
}


# Validate directory structures
check_directory "Assets/_Core/Scripts" "$SCRIPT_EXT" "Core Scripts"
check_directory "Assets/_Art/Models" "$MODEL_EXT" "Art Models"
check_directory "Assets/_Art/Textures" "$TEXTURE_EXT" "Art Textures"
check_directory "Assets/_Art/Material" "$MATERIAL_EXT" "Art Materials"
check_directory "Assets/_Art/Terrain" "$TERRAIN_EXT" "Art Terrain"
check_directory "Assets/_Art/Prefabs" "$UNITY_ASSET_EXT" "Art Prefabs"
check_directory "Assets/_Core/Prefabs" "$UNITY_ASSET_EXT" "Core Prefabs"
check_directory "Assets/_Scenes" "$SCENE_EXT" "Scenes"

# Success message
echo "All checked directories have the correct structure and file formats."
