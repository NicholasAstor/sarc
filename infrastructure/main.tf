variable "db_password" {
  description = "Password for the RDS instance"
  type        = string
  sensitive   = true
}

variable "key_pair_name" {
  description = "Name of the AWS key pair for EC2 access"
  type        = string
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
    db_username = "admin"
    db_password = var.db_password
    rds_security_group_id = module.security_groups.rds_sg_id
    private_subnet_ids = module.vpc.private_subnet_ids
}

module "ec2" {
    source = "./modules/ec2"
    
    instance_type = "t3.micro"
    key_pair_name = var.key_pair_name
    public_subnet_id = module.vpc.public_subnet_id
    ec2_security_group_id = module.security_groups.ec2_sg_id
    
    # Database connection info
    db_endpoint = module.database.db_endpoint
    db_name = "sarc_db"
    db_username = "admin"
    db_password = var.db_password
}