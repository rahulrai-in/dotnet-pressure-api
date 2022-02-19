#Win: set KUBECONFIG=D:\Projects\dotnet-pressure-api\tests\my-linode-cluster-kubeconfig.yaml
#Shell KUBECONFIG=...

# HPA test: Install HPA and execute the following commands

kubectl run -i --tty mem-load-gen --rm --image=busybox --restart=Never -- /bin/sh -c "wget -q -O- --post-data= http://pressure-api-service/memory/2000/duration/60"

kubectl get hpa pressure-api-hpa --watch

kubectl get deployment pressure-api-deployment --watch

kubectl delete hpa/pressure-api-hpa

# VPA test: Install VPA and execute the following commands
kubectl run -i --tty mem-load-gen --rm --image=busybox --restart=Never -- /bin/sh -c "wget -q -O- http://pressure-api-service/cpu/10/duration/180"

kubectl get pods -l app=pressure-api

kubectl describe pod pressure-api-8f8f8f8f8f8f

# Cluster Proportional Autoscaler test: Install CPA and execute the following commands
https://github.com/kubernetes-sigs/cluster-proportional-autoscaler

helm repo add cluster-proportional-autoscaler https://kubernetes-sigs.github.io/cluster-proportional-autoscaler
helm repo update

# Cluster Autoscaling
