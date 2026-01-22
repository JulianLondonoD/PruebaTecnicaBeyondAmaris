#!/bin/bash
# TodoApp Frontend Quick Start Script

echo "========================================="
echo " TodoApp Frontend - Quick Start"
echo "========================================="
echo ""

# Check if we're in the frontend directory
if [ ! -f "package.json" ]; then
    echo "❌ Error: Please run this script from the frontend directory"
    exit 1
fi

# Check if node_modules exists
if [ ! -d "node_modules" ]; then
    echo "📦 Installing dependencies..."
    npm install
    echo ""
fi

echo "✅ Dependencies ready"
echo ""
echo "Choose an action:"
echo "1) Start development server (port 3000)"
echo "2) Run tests"
echo "3) Build for production"
echo "4) Preview production build"
echo "5) Exit"
echo ""
read -p "Enter your choice (1-5): " choice

case $choice in
    1)
        echo ""
        echo "🚀 Starting development server..."
        echo "📍 http://localhost:3000"
        echo ""
        npm run dev
        ;;
    2)
        echo ""
        echo "🧪 Running tests..."
        npm run test
        ;;
    3)
        echo ""
        echo "🏗️  Building for production..."
        npm run build
        ;;
    4)
        echo ""
        echo "👀 Previewing production build..."
        npm run preview
        ;;
    5)
        echo "Goodbye!"
        exit 0
        ;;
    *)
        echo "Invalid choice. Please run the script again."
        exit 1
        ;;
esac
