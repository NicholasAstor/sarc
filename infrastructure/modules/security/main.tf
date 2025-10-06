# Security Group for EC2 (Web Application)
resource "aws_security_group" "ec2_sg" {
  name        = "ec2-web-sg"
  description = "Security group for EC2 web application"
  vpc_id      = var.vpc_id

  # HTTP access
  ingress {
    from_port   = 80
    to_port     = 80
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  # HTTPS access
  ingress {
    from_port   = 443
    to_port     = 443
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  # SSH access
  ingress {
    from_port   = 22
    to_port     = 22
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]  # In production, restrict this to your IP
  }

  # Custom application port (for .NET API)
  ingress {
    from_port   = 5000
    to_port     = 5000
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  # HTTPS for .NET API
  ingress {
    from_port   = 5001
    to_port     = 5001
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  # All outbound traffic
  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  tags = {
    Name        = "ec2-web-sg"
    Environment = "development"
    Terraform   = "true"
  }
}

# Security Group for RDS PostgreSQL
resource "aws_security_group" "rds_sg" {
  name        = "rds-postgres-sg"
  description = "Security group for RDS PostgreSQL database"
  vpc_id      = var.vpc_id

  # PostgreSQL access from EC2 security group
  ingress {
    from_port       = 5432
    to_port         = 5432
    protocol        = "tcp"
    security_groups = [aws_security_group.ec2_sg.id]
  }

  # All outbound traffic
  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  tags = {
    Name        = "rds-postgres-sg"
    Environment = "development"
    Terraform   = "true"
  }
}
