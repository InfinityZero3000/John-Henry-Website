#!/bin/bash

# Script to add @model directive to all remaining views
# Run from project root directory

echo "Adding @model directive to Freelancer views..."

# Array of files to update
files=(
    "Views/Home/FreelancerDress.cshtml"
    "Views/Home/FreelancerShirt.cshtml"
    "Views/Home/FreelancerTrousers.cshtml"
    "Views/Home/FreelancerSkirt.cshtml"
    "Views/Home/FreelancerAccessories.cshtml"
)

# Model directive to add
model_directive="@model List<JohnHenryFashionWeb.Models.Product>"

for file in "${files[@]}"; do
    if [ -f "$file" ]; then
        echo "Processing $file..."
        # Check if file already has @model
        if ! grep -q "@model" "$file"; then
            # Add @model at the beginning
            echo "$model_directive" | cat - "$file" > temp && mv temp "$file"
            echo "✅ Added @model to $file"
        else
            echo "⚠️  $file already has @model"
        fi
    else
        echo "❌ File not found: $file"
    fi
done

echo ""
echo "✅ Done! All files have been updated with @model directive."
echo ""
echo "NOTE: You still need to replace the hard-coded product HTML with dynamic @foreach loop."
echo "Refer to JohnHenryShirt.cshtml or JohnHenry.cshtml for the correct pattern."
