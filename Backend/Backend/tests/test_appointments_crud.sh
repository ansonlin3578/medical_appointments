#!/bin/bash

BASE_URL="http://localhost:5000/api/appointments"

echo "📥 Creating appointment..."
RESPONSE=$(curl -s -X POST $BASE_URL \
  -H "Content-Type: application/json" \
  -d '{
    "name": "SungYu Lin",
    "date": "2025-04-22",
    "startTime": "09:00:00",
    "endTime": "10:00:00",
    "doctorId": "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"
  }')

echo "🔁 Response:"
echo "$RESPONSE"

ID=$(echo $RESPONSE | grep -oE '[0-9a-f-]{36}')

echo ""
echo "✅ Created appointment with ID: $ID"

echo ""
echo "🔍 Getting all appointments..."
curl -s $BASE_URL | jq

echo ""
echo "🛠 Updating appointment..."
curl -s -X PUT $BASE_URL/$ID \
  -H "Content-Type: application/json" \
  -d '{
    "id": "'"$ID"'",
    "name": "SungYu Lin (Updated)",
    "date": "2025-04-22",
    "startTime": "10:00:00",
    "endTime": "11:00:00",
    "doctorId": "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"
  }'

echo ""
echo "🔁 Getting updated appointment..."
curl -s $BASE_URL/$ID | jq

echo ""
echo "❌ Deleting appointment..."
curl -s -X DELETE $BASE_URL/$ID

echo ""
echo "🧹 Final list of appointments:"
curl -s $BASE_URL | jq
