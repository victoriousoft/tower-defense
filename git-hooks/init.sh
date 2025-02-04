for file in $(ls ./git-hooks | grep -v '\.sh$'); do
    chmod +x ./git-hooks/$file
    [ -L ".git/hooks/$file" ] && rm .git/hooks/$file
    ln -s ./git-hooks/$file .git/hooks/$file    
    echo "Initialized $file hook"
done
