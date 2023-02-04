import { useState, useEffect } from "react";
import { getData, postData, getDataCustom } from "../Common/FuncAxios";
import { useForm } from "react-hook-form";
import CheckboxTree from "react-checkbox-tree";
import "react-checkbox-tree/lib/react-checkbox-tree.css";
import { ToastError } from "../Common/FuncToast";
import LoadingPage from "../Common/Loading/LoadingPage";

const SetCusForUser = (props) => {
  const { selectIdClick, hideModal } = props;
  const {
    register,
    reset,
    formState: { errors },
    handleSubmit,
  } = useForm({
    mode: "onChange",
  });

  const [IsLoading, SetIsLoading] = useState(true);
  const [checked, setChecked] = useState([]);
  const [expanded, setExpanded] = useState([]);
  const [nodes, setNodes] = useState([]);

  useEffect(() => {
    if (selectIdClick && Object.keys(selectIdClick).length > 0) {
      SetIsLoading(true);
      (async () => {
        let tree = await getData(
          `User/LoadTreeCustomer?userid=${selectIdClick.id}`
        );

        setNodes(tree.listTree);

        if (tree.isChecked && tree.isChecked.length > 0) {
          setChecked(tree.isChecked);
        } else {
          setChecked([]);
        }

        SetIsLoading(false);
      })();
    }
  }, [selectIdClick]);

  const onSubmit = async (data) => {
    if (!checked || checked.length < 1) {
      ToastError("Vui Lòng Chọn Khách Hàng");
      return;
    }
    SetIsLoading(true);

    const setCus = await postData("User/SetCusForUser", {
      UserID: selectIdClick.id,
      CusId: checked,
    });

    if (setCus === 1) {
      hideModal();
    }
    SetIsLoading(false);
  };

  return (
    <div className="card card-primary">
      <div>
        {IsLoading === true && (
          <div>
            <LoadingPage></LoadingPage>
          </div>
        )}
      </div>

      {IsLoading === false && (
        <form onSubmit={handleSubmit(onSubmit)}>
          <div className="card-body">
            <div className="row">
              <div className="form-group">
                <label htmlFor="TrangThai">Phân Khách Hàng(*)</label>
                <CheckboxTree
                  iconsClass="fa5"
                  nodes={nodes}
                  checked={checked}
                  expanded={expanded}
                  showExpandAll={true}
                  showNodeIcon={true}
                  onCheck={(checked) => setChecked(checked)}
                  onExpand={(expanded) => setExpanded(expanded)}
                />
              </div>
            </div>
          </div>
          <div className="card-footer">
            <div>
              <button
                type="submit"
                className="btn btn-primary"
                style={{ float: "right" }}
              >
                Xác Nhận
              </button>
            </div>
          </div>
        </form>
      )}
    </div>
  );
};

export default SetCusForUser;
