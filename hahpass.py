import bcrypt

# Mật khẩu cần hash
password = "newpassword123"

# Tạo hash bằng bcrypt
# salt tự động được tạo với work factor 12 (tương thích với BCrypt.Net)
hashed_password = bcrypt.hashpw(password.encode('utf-8'), bcrypt.gensalt(rounds=12))

# In ra mật khẩu và hash
print(f"Password: {password}")
print(f"Hash: {hashed_password.decode('utf-8')}")