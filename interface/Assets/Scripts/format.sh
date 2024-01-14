for i in {1..3}
do
find . -iname "*.cs"   \
   | xargs clang-format -i
done
echo "Done!"
