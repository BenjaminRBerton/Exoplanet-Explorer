#!/bin/bash

# Test script for Gemini API integration

echo "=== Gemini API Test Script ==="
echo ""

# Check if API key is provided
if [ -z "$1" ]; then
    echo "Usage: ./test_gemini_api.sh YOUR_API_KEY"
    echo ""
    echo "This will test your Gemini API key with a simple request."
    exit 1
fi

API_KEY="$1"
API_URL="https://generativelanguage.googleapis.com/v1beta/models/gemini-3.8-flash:generateContent"

# Test request
echo "Testing API key..."
echo ""

if ! RESPONSE=$(curl --silent --show-error --fail-with-body -X POST "$API_URL" \
    -H "x-goog-api-key: $API_KEY" \
    -H "Content-Type: application/json" \
    -d '{
        "contents": [{
            "parts": [{
                "text": "Hello, respond with a simple greeting."
            }]
        }]
    }'); then
    echo "❌ Gemini API request failed"
    exit 1
fi

# Check if response contains error
if echo "$RESPONSE" | grep -q '"error"'; then
    echo "❌ API key test FAILED"
    echo ""
    echo "Error response:"
    if command -v jq >/dev/null 2>&1; then
        echo "$RESPONSE" | jq '.'
    else
        echo "$RESPONSE"
    fi
    exit 1
else
    echo "✅ API key test SUCCESSFUL"
    echo ""
    echo "Response:"
    if command -v jq >/dev/null 2>&1; then
        echo "$RESPONSE" | jq -r '.candidates[0].content.parts[0].text'
    else
        echo "$RESPONSE"
    fi
fi

echo ""
echo "Your API key is valid and ready to use!"
echo ""
echo "Next steps:"
echo "1. Save your API key using one of the methods in GEMINI_SETUP.md"
echo "2. Launch the game and try the path optimization feature"
