import axios from "axios";
import { ToastSuccess, ToastError, ToastWarning } from "../Common/FuncToast";

const getData = async (url) => {
  const get = await axios.get(url);
  var data = get.data;
  return data;
};

const postData = async (url, data, header = null) => {
  var isSuccess = 0;
  await axios
    .post(url, data, header)
    .then((response) => {
      ToastSuccess(`${response.data}`);
      return (isSuccess = 1);
    })
    .catch((error) => {
      ToastError(`${error.response.data}`);
      return (isSuccess = 0);
    });

  return isSuccess;
};

const postFile = async (url, data) => {
  var isSuccess = 0;

  await axios
    .post(url, data, {
      headers: { "Content-Type": "multipart/form-data" },
    })
    .then((response) => {
      ToastSuccess(`${response.data}`);
      return (isSuccess = 1);
    })
    .catch((error) => {
      ToastError(`${error.response.data}`);
      return (isSuccess = 0);
    });
  return isSuccess;
};

const putData = async (url, data, header = null) => {
  var isSuccess = 0;
  await axios
    .put(url, data, header)
    .then((response) => {
      ToastSuccess(`${response.data}`);
      return (isSuccess = 1);
    })
    .catch((error) => {
      ToastError(`${error.response.data}`);
      return (isSuccess = 0);
    });

  return isSuccess;
};

const deleteData = async (url) => {
  var isSuccess = 0;
  axios.delete(url).then(
    (response) => {
      ToastSuccess(`${response.data}`);
      return (isSuccess = 1);
    },
    (error) => {
      ToastSuccess(`${error.response.data}`);
      return (isSuccess = 0);
    }
  );
  return isSuccess;
};

export { getData, postData, putData, deleteData, postFile };
