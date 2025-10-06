#!/bin/bash
yum update -y

# Install Docker
amazon-linux-extras install docker -y
systemctl start docker
systemctl enable docker
usermod -a -G docker ec2-user

# Install Docker Compose
curl -L "https://github.com/docker/compose/releases/download/1.29.2/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
chmod +x /usr/local/bin/docker-compose

# Install .NET Runtime
rpm -Uvh https://packages.microsoft.com/config/centos/7/packages-microsoft-prod.rpm
yum install -y aspnetcore-runtime-8.0

# Create application directory
mkdir -p /opt/sarc

# Create environment file for database connection
cat > /opt/sarc/.env << EOF
DB_HOST=${db_endpoint}
DB_NAME=${db_name}
DB_USER=${db_username}
DB_PASSWORD=${db_password}
DB_PORT=5432
EOF

# Create a simple startup script
cat > /opt/sarc/start.sh << 'EOF'
#!/bin/bash
cd /opt/sarc
# Add your application startup commands here
# For example: dotnet YourApp.dll
EOF

chmod +x /opt/sarc/start.sh

# Set up CloudWatch agent (optional)
yum install -y amazon-cloudwatch-agent

echo "EC2 instance setup completed"
