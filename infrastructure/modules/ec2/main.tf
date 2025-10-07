# Data source to get the latest Amazon Linux 2 AMI
data "aws_ami" "amazon_linux" {
  most_recent = true
  owners      = ["amazon"]

  filter {
    name   = "name"
    values = ["amzn2-ami-hvm-*-x86_64-gp2"]
  }

  filter {
    name   = "virtualization-type"
    values = ["hvm"]
  }
}

# EC2 Instance
resource "aws_instance" "web_server" {
  ami                    = data.aws_ami.amazon_linux.id
  instance_type          = var.instance_type
  key_name              = var.key_pair_name
  subnet_id             = var.public_subnet_id
  vpc_security_group_ids = [var.ec2_security_group_id]
  
  associate_public_ip_address = true

  tags = {
    Name        = "sarc-web-server"
    Environment = "development"
    Terraform   = "true"
  }
}
