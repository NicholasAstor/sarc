variable "db_password" {
  description = "Password for the RDS instance"
  type        = string
  sensitive   = true
}

variable "key_pair_name" {
  description = "Name for the EC2 key pair"
  type        = string
  default     = "sarc-key-pair"
}

provider "aws" {
  region = "us-east-1"  # north virginia because cheap
}

# Generate a new key pair
resource "aws_key_pair" "main" {
  key_name   = var.key_pair_name
  public_key = tls_private_key.main.public_key_openssh

  tags = {
    Name = var.key_pair_name
    Terraform = "true"
  }
}

# Generate the private key
resource "tls_private_key" "main" {
  algorithm = "RSA"
  rsa_bits  = 4096
}

# Save the private key locally (optional - for SSH access)
resource "local_file" "private_key" {
  content  = tls_private_key.main.private_key_pem
  filename = "${path.module}/sarc-private-key.pem"
  file_permission = "0600"
}

module "vpc" {
    source = "./modules/networking"
}

module "security_groups" {
    source = "./modules/security"
    vpc_id = module.vpc.vpc_id
}

module "database" {
    source = "./modules/database"

    db_name = "sarc_db"
    db_username = "sarc_user"
    db_password = var.db_password
    rds_security_group_id = module.security_groups.rds_sg_id
    private_subnet_ids = module.vpc.private_subnet_ids
}

module "ec2" {
    source = "./modules/ec2"
    
    instance_type = "t3.micro"
    key_pair_name = aws_key_pair.main.key_name
    public_subnet_id = module.vpc.public_subnet_id
    ec2_security_group_id = module.security_groups.ec2_sg_id
    
    # Database connection info
    db_endpoint = module.database.db_endpoint
    db_name = "sarc_db"
    db_username = "sarc_user"
    db_password = var.db_password
    
    depends_on = [aws_key_pair.main]
}